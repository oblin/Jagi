using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace JagiCore.Helpers
{
    /// <summary>
    /// 將 CSV 轉成 T class 的 object list，每一個 T 只能有一個對應方式
    /// </summary>
    /// <typeparam name="T">CSV 轉成的 T Type</typeparam>
    public class CsvTools<T>
    {
        private static CsvTools<T> _csvObject;

        private CsvTools() : this(string.Empty) { }

        private CsvTools(string delimiter)
        {
            Configuration = new Configuration();
            // 設定目前的文化與編碼，這跟實際結果無關，但設定 utf-8 在計算長度有關
            Configuration.CultureInfo = new CultureInfo("zh-TW");
            Configuration.Encoding = Encoding.UTF8;
            Configuration.TrimOptions = TrimOptions.Trim;
            if (!string.IsNullOrEmpty(delimiter))
                Configuration.Delimiter = delimiter;
        }

        /// <summary>
        /// 預設為全域變數，與其他執行序共用，因此可能會有舊有的 Maps
        /// </summary>
        public static CsvTools<T> Default = _csvObject ?? (_csvObject = new CsvTools<T>());

        /// <summary>
        /// 獨立的 CsvHelper 個體，與全域的 Default 變更
        /// 如果擔心是否已有設定檔，可透過此方式建立新的個體
        /// </summary>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static CsvTools<T> Create(string delimiter = null)
        {
            return new CsvTools<T>(delimiter);
        }

        private Configuration Configuration { get; }

        /// <summary>
        /// 將 csv file 自動轉成 T list
        /// </summary>
        /// <param name="fullFileName">CVS 所在的全路徑</param>
        /// <returns></returns>
        public IEnumerable<T> ReadFromFile(string fullFileName)
        {
            return ExecuteReadFromFile(fullFileName, () =>
            {
                SetupCsvMapping();
            });
        }

        /// <summary>
        /// 將 csv file 依據指定的 CsvClassMap 方式轉成 T List
        /// </summary>
        /// <typeparam name="S">繼承 CsvClassMap<T> 的 Converter</typeparam>
        /// <param name="fullFileName">CVS 所在的全路徑</param>
        /// <returns></returns>
        public IEnumerable<T> ReadFromFile<S>(string fullFileName)
            where S : ClassMap, new()
        {
            var customMap = new S();

            return ExecuteReadFromFile(fullFileName, () =>
            {
                SetupCsvMapping(customMap);
            });
        }

        /// <summary>
        /// 將 T list 寫入到 CSV File，標題就是 T 的屬性名稱；此呼叫會刪除現存檔案，重新產生
        /// </summary>
        /// <param name="location">寫入的檔案全路徑</param>
        /// <param name="records">要寫入的物件陣列</param>
        public void WriteToFile(string location, IEnumerable<T> records)
        {
            ExecuteWriteToFile(location, records, () =>
            {
                SetupCsvMapping();
            });
        }

        /// <summary>
        /// 將 T 單筆資料寫入到 CSV File，此寫法會新增到既有的檔案後面
        /// </summary>
        /// <param name="location">寫入的檔案全路徑</param>
        /// <param name="records">要寫入的物件陣列</param>
        public void WriteToFile(string location, T record)
        {
            ExecuteWriteToFile(location, record, () =>
            {
                SetupCsvMapping();
            });
        }

        /// <summary>
        /// 將 T list 寫入到 CSV File，寫入的方式與標題依據 S 設定的內容而定
        /// </summary>
        /// <typeparam name="S">設定寫入 CSV 的方式</typeparam>
        /// <param name="location">寫入的檔案全路徑</param>
        /// <param name="records">要寫入的物件陣列</param>
        public void WriteToFile<S>(string location, IEnumerable<T> records)
            where S : ClassMap, new()
        {
            var customMap = new S();

            ExecuteWriteToFile(location, records, () =>
            {
                SetupCsvMapping(customMap);
            });
        }

        private IEnumerable<T> ExecuteReadFromFile(string fullFileName, Action setupMapping)
        {
            if (!File.Exists(fullFileName))
                throw new FileNotFoundException();

            setupMapping();

            return CsvFileToObjectList(fullFileName);
        }

        private void ExecuteWriteToFile(string location, IEnumerable<T> records, Action setupMapping)
        {
            if (records == null || !records.Any())
                throw new NullReferenceException();

            setupMapping();

            RecordsToCsvFile(location, records);
        }

        private void ExecuteWriteToFile(string location, T record, Action setupMapping)
        {
            if (EqualityComparer<T>.Default.Equals(record, default(T)))
                throw new NullReferenceException();

            setupMapping();

            RecordsToCsvFile(location, record);
        }

        /// <summary>
        /// 移除所有已經註冊的 Types，之後就會重新再進行註冊
        /// 主要用途僅在於測試時候使用
        /// </summary>
        public void ClearAllRegisteredTypes()
        {
            Configuration.UnregisterClassMap();
        }

        private void RecordsToCsvFile(string location, IEnumerable<T> records)
        {
            if (File.Exists(location))
                File.Delete(location);

            using (var fs = File.Create(location))
            using (var stream = new StreamWriter(fs, Encoding.UTF8))
            using (var csv = new CsvWriter(stream, Configuration))
            {
                csv.WriteRecords(records);
            }
        }

        /// <summary>
        /// 將 record 記錄到檔案中；如果已經存在就繼續往下寫，否則就產生新的
        /// </summary>
        /// <param name="location">檔案目錄</param>
        /// <param name="record"></param>
        private void RecordsToCsvFile(string location, T record)
        {
            // TODO: 仍然使用 WriteRecords 因為 WriteRecord 有問題，無法正確寫入
            var records = new List<T> { record };
            if (File.Exists(location))
            {
                using (var fs = new FileStream(location, FileMode.Append, FileAccess.Write))
                using (var stream = new StreamWriter(fs, Encoding.UTF8))
                using (var csv = new CsvWriter(stream, Configuration))
                {
                    csv.WriteRecords(records);
                }
            }
            else
            {
                using (var fs = File.Create(location))
                using (var stream = new StreamWriter(fs, Encoding.UTF8))
                using (var csv = new CsvWriter(stream, Configuration))
                {
                    csv.WriteRecords(records);
                }
            }
        }

        private void WriteRecord(T records, FileStream fs)
        {
            using (var stream = new StreamWriter(fs, Encoding.UTF8))
            using (var csv = new CsvWriter(stream, Configuration))
            {
                csv.WriteRecord(records);
            }
        }

        //private void WriteRecord(T record, FileStream fs)
        //{
        //    using (var stream = new StreamWriter(fs, Encoding.UTF8))
        //    using (var csv = new CsvWriter(stream, Configuration))
        //    {
        //        csv.WriteRecord<T>(record);
        //    }
        //}

        public bool IsMapDefined()
        {
            return IsMapDefined<T>();
        }

        public bool IsMapDefined<S>()
        {
            return Configuration.Maps.Find<S>() != null;
        }

        private IEnumerable<T> CsvFileToObjectList(string fullFileName)
        {
            using (var fs = File.OpenRead(fullFileName))
            // 如果有中文字，文字檔案必須要是 UTF8 編碼，不可以是 ASCII 
            using (var stream = new StreamReader(fs, Encoding.UTF8))
            using (var csv = new CsvReader(stream, Configuration))
            {
                List<T> resultSet = new List<T>();
                while (csv.Read())
                {
                    var item = csv.GetRecord<T>();
                    resultSet.Add(item);
                }
                return resultSet;
            }
        }

        /// <summary>
        /// 建立属于 T 的 mapping 方式，如果有 CsvClassMap<T> 的方案，則傳入 type converter object
        /// 否則會自動建立
        /// 請注意，自動建立時，必須要完全一樣的欄位，否則會出錯。
        /// </summary>
        /// <param name="customMap">自訂的 converter，如果沒有就用 Auto map 方案</param>
        private void SetupCsvMapping(ClassMap customMap = null)
        {
            // 每一個 T 只能有一個對應方式
            if (!IsMapDefined())
            {
                // AutoMap<T> 依據 CSV 第一行的欄位名稱對應 T 的欄位名稱
                var map = customMap ?? Configuration.AutoMap<T>();
                Configuration.RegisterClassMap(map);
            }
        }
    }

    public class DateConverter : DefaultTypeConverter
    {
        public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
        {
            if (value != null)
            {
                DateTime date = Convert.ToDateTime(value);
                string dateString = date.ToString("yyyy/MM/dd");
                value = dateString;
                return value.ToString();
            }
            else
            {
                return base.ConvertToString(value, row, memberMapData);
            }
        }
    }
}
