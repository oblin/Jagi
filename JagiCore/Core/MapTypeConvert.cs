using AutoMapper;
using System;

namespace JagiCore
{
    /// <summary>
    /// 設定 AutoMaper 將 Nullabl integer 轉換成數字
    /// </summary>
    public class NullableIntToString : ITypeConverter<int?, string>
    {
        public string Convert(int? source, string destination, ResolutionContext context)
        {
            if (source.HasValue)
                return ((int)source).ToString();
            else
                return string.Empty;
        }
    }

    public class IntToString : ITypeConverter<int, string>
    {
        public string Convert(int source, string destination, ResolutionContext context)
        {
            return source.ToString();
        }
    }

    public class StringToInt : ITypeConverter<string, int>
    {
        public int Convert(string source, int destination, ResolutionContext context)
        {
            return System.Convert.ToInt16(source);
        }
    }

    /// <summary>
    /// 設定 AutoMaper 將文字轉換成 Nullabl decimal
    /// </summary>
    public class StringToDecimalNull : ITypeConverter<string, decimal?>
    {
        public decimal? Convert(string source, decimal? destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            else
                return System.Convert.ToDecimal(source);
        }
    }

    /// <summary>
    /// 設定 AutoMaper 將文字轉換成 Nullabl DateTime
    /// </summary>
    public class StringToDateTimeNull : ITypeConverter<string, DateTime?>
    {
        public DateTime? Convert(string source, DateTime? destination, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source))
                return null;
            else
                return System.Convert.ToDateTime(source, System.Globalization.CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    /// 設定 AutoMaper 將文字轉換成 Nullabl DateTime
    /// </summary>
    public class DateTimeNullToString : ITypeConverter<DateTime?, string>
    {
        public string Convert(DateTime? source, string destination, ResolutionContext context)
        {
            if (source.HasValue)
                return ((DateTime)source).ToString("yyyy/MM/dd");
            else
                return string.Empty;
        }
    }

    /// <summary>
    /// 設定 AutoMaper 將文字轉換成 Nullabl DateTime
    /// </summary>
    public class DateTimeToString : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            return source.ToString("yyyy/MM/dd");
        }
    }
}
