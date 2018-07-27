using System;

namespace JagiCore.Helpers
{
    public static class DateTimeHelper
    {
        public static string ToTaiwanString(this DateTime date, bool withoutSlash = false)
        {
            if (date.Year < 1911)
                throw new ArgumentOutOfRangeException("DateTime.ToTaiwanString 年度不可以小於 1911");

            int year = Convert.ToInt16(date.AddYears(-1911).Year);

            if (withoutSlash)
                return year.ToString().PadLeft(3, '0') + date.Month.ToString().PadLeft(2, '0') + date.Day.ToString().PadLeft(2, '0');
            else
                return year.ToString().PadLeft(3, '0') + "/"
                + date.Month.ToString().PadLeft(2, '0') + "/"
                + date.Day.ToString().PadLeft(2, '0');
        }

        public static string ToCommonString(this DateTime? date)
        {
            return date == null ?
                    string.Empty :
                    ToCommonString((DateTime)date);
        }

        public static string ToCommonString(this DateTime date)
        {
            return date.ToString("yyyy/MM/dd");
        }

        public static string ToString(this DateTime date)
        {
            return ToCommonString(date);
        }

        /// <summary>
        /// 日期必須要用字串方式寫入資料庫，此函數包含時間
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ToDatabaseString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 將日期字串轉換成標準日期字串，例如： 2017-1-12 下午 12:00:00 轉成： 2017/01/12
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string ToDateString(this string date)
        {
            DateTime result;
            if (DateTime.TryParse(date, out result))
                return result.ToCommonString();
            return string.Empty;
        }

        /// <summary>
        /// 將民國年 1070101 轉成日期格式
        /// </summary>
        /// <param name="dateString">必須要是七碼日期</param>
        /// <returns></returns>
        public static DateTime? ConvertChineseToDateTime(this string dateString)
        {
            if (string.IsNullOrEmpty(dateString) || dateString.Length != 7)
                return null;

            int year = Convert.ToInt16(dateString.Substring(0, 3)) + 1911;
            int month = Convert.ToInt16(dateString.Substring(3, 2));
            int day = Convert.ToInt16(dateString.Substring(5, 2));
            return new DateTime(year, month, day);
        }
    }
}
