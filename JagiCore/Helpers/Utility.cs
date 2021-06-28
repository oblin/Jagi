using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;

namespace JagiCore.Helpers
{
    public static class Utility
    {
        public static TimeSpan Timing(Action toTime)
        {
            var timer = Stopwatch.StartNew();
            toTime();
            timer.Stop();
            return timer.Elapsed;
        }

        public static string ConvertToUtf8(this string subject)
        {
            if (string.IsNullOrEmpty(subject))
                return subject;

            UTF8Encoding utf8 = new UTF8Encoding();

            byte[] encodedBytes = utf8.GetBytes(subject);

            return Convert.ToString(encodedBytes);
        }

        public static string ConvertToUtf8(this byte[] bytes)
        {
            var byteUtf8Name = Encoding.Convert(Encoding.Default, Encoding.UTF8, bytes);
            return Encoding.UTF8.GetString(byteUtf8Name);
        }

        public static int ParsingOracleNumberEndChar(string numberString)
        {
            if (string.IsNullOrEmpty(numberString))
                return 0;

            var last = numberString.Substring(numberString.Length - 1, 1);
            int temp;
            if (int.TryParse(last, out temp))
            {
                return Convert.ToInt32(numberString);
            }
            else
            {
                return Convert.ToInt32(numberString.Substring(0, numberString.Length - 1));
            }
        }

        private static Regex rx = new Regex("^[\u4e00-\u9fa5]$");
        private static Regex rx2 = new Regex("^[\u0000-\u00ff]$");

        public static string TrimOracleEndNotChineseChar(string oracleChineseString)
        {
            if (string.IsNullOrEmpty(oracleChineseString))
                return oracleChineseString;

            var endWord = oracleChineseString.Substring(oracleChineseString.Length - 1, 1);
            if (rx.IsMatch(endWord))
                return oracleChineseString;
            else
                return oracleChineseString.Substring(0, oracleChineseString.Length - 1);
        }

        public static string TrimOracleEndNotAsciiChar(string oracleAsciiString)
        {
            if (string.IsNullOrEmpty(oracleAsciiString))
                return oracleAsciiString;

            StringBuilder sb = new StringBuilder(oracleAsciiString.Length);
            foreach (char c in oracleAsciiString)
            {
                if ((int)c > 127) // you probably don't want 127 either
                    continue;
                if ((int)c < 32)  // I bet you don't want control characters 
                    continue;
                if (c == ',')
                    continue;
                if (c == '"')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 16位：ComputeHash， For 傳統的 FormsAuthentication.HashPasswordForStoringInConfigFile(Request["PASSWORD"], "MD5")
        /// 加密機制
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();

            byte[] myData = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < myData.Length; i++)
            {
                sBuilder.Append(myData[i].ToString("x"));
            }

            return sBuilder.ToString().ToUpper();
        }

        // Extension method, call for any object, eg "if (x.IsNumeric())..."
        public static bool IsNumeric(this object x)
        {
            return x == null ? false : IsNumeric(x.GetType());
        }

        // Method where you know the type of the object
        public static bool IsNumeric(Type type)
        {
            return IsNumeric(type, Type.GetTypeCode(type));
        }

        // Method where you know the type and the type code of the object
        public static bool IsNumeric(Type type, TypeCode typeCode)
        {
            return typeCode == TypeCode.Decimal || (type.IsPrimitive && typeCode != TypeCode.Object && typeCode != TypeCode.Boolean && typeCode != TypeCode.Char);
        }

        public static D Map<S, D>(this D destination, S source)
        {
            return Mapper.Map(source, destination);
        }

        /// <summary>
        /// 將 PascalCase 轉為 Snake Case: BirthDate => birth_date
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToSnakeCase(this string input)
        {
            if (string.IsNullOrEmpty(input)) { return input; }

            var startUnderscores = Regex.Match(input, @"^_+");
            return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }
    }
}
