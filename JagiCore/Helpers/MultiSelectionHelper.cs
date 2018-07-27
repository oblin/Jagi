using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace JagiCore.Helpers
{
    /// <summary>
    /// 主要目的為提供 string Property 可以提供多個選項的值
    /// 例如：    public string Selections = "1-2-4-5"(總共有 1, 2, 3, 4, 5, 6 個選項)
    /// 這就代表本筆資料選擇了 1,2,4,5
    /// 透過 SetJsonBooleanProperty 可以跟 AutoMap 合作，將 Selection 變成 Selection1, Selection2... 
    /// 這樣的多項屬性，方便用來跟前端的欄位對應。
    /// </summary>
    public static class MultiSelectionHelper
    {
        private const char _seperator = '-';

        public static void SetMultiBooleanProperty(this object dest, Expression<Func<string>> action, string selection)
        {
            if (string.IsNullOrEmpty(selection))
                return;
            if (!selection.Contains('-'))
                throw new InvalidOperationException("必須要使用 - 分隔！");

            var expression = (MemberExpression)action.Body;
            if (expression == null)
                throw new NullReferenceException("無法使用的 expression action");

            var name = expression.Member.Name;
            var options = selection.Split(_seperator);
            SetJsonBooleanProperty(dest, name, options);
        }

        public static void SetJsonBooleanProperty(this object dest, string name, string[] options)
        {
            foreach (var opt in options)
            {
                if (string.IsNullOrEmpty(opt))
                    continue;
                var propName = name + opt;
                PropertyInfo prop = dest.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
                if (null != prop && prop.CanWrite && prop.PropertyType == typeof(bool))
                {
                    prop.SetValue(dest, true, null);
                }
                else
                {
                    throw new InvalidOperationException($"無法轉換資料, Name: {name}, Value: {string.Join(",", options)}");
                }
            }
        }

        public static string ComposeSelection(this object source, Expression<Func<string>> action)
        {
            var expression = (MemberExpression)action.Body;
            if (expression == null)
                throw new NullReferenceException("無法使用的 expression action");

            var name = expression.Member.Name;
            string result = string.Empty;

            Type type = source.GetType();
            foreach(var prop in type.GetProperties())
            {
                if (prop.Name.StartsWith(name))
                {
                    if (Convert.ToBoolean(prop.GetValue(source, null)))
                    {
                        string temp = prop.Name.TrimStart(name.ToCharArray());
                        if (!string.IsNullOrEmpty(temp))
                        {
                            result += temp + "-";
                        }
                    }
                }
            }
            return result.TrimEnd('-');
        }
    }
}
