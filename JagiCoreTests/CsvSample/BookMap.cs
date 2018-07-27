using CsvHelper.Configuration;
using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCoreTests.CsvSample
{
    public sealed class Book1Map : ClassMap<Book1>
    {
        public Book1Map()
        {
            // 依據 CSV 的欄位順序指定對應的 Class Properties
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Description).Index(2);
        }
    }

    public sealed class Book2Map : ClassMap<Book2>
    {
        public Book2Map()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Description).Index(2);
            // 注意，加入 Name 的設定，在 Excel 必須要完全一樣（不可以有空白）
            Map(m => m.Date).Index(3).Name("日期");
        }
    }

    public sealed class Book4DateConvertMap : ClassMap<Book4>
    {
        public Book4DateConvertMap()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Description).Index(2);
            // 注意，加入 Name 的設定，在 Excel 必須要完全一樣（不可以有空白）
            Map(m => m.Date).Index(3).Name("日期").TypeConverter<DateConverter>();
        }
    }

    public sealed class Book3Map : ClassMap<Book3>
    {
        public Book3Map()
        {
            Map(m => m.Id).Index(0);
            Map(m => m.Name).Index(1);
            Map(m => m.Description).Index(2);
            Map(m => m.Date).Index(3);
        }
    }
}
