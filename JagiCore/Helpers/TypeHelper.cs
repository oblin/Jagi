using AutoMapper;
using JagiCore.Angular;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JagiCore.Helpers
{
    public static class TypeHelper
    {
        public static Dictionary<string, object> DynamicToDictionary(dynamic dynamicObject)
        {
            var dictionary = new Dictionary<string, object>();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynamicObject))
            {
                object obj = propertyDescriptor.GetValue(dynamicObject);
                dictionary.Add(propertyDescriptor.Name, obj);
            }
            return dictionary;
        }

        public static ExpandoObject ToExpando<T>(this T anonymousObject) where T : class
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(anonymousObject))
            {
                var obj = propertyDescriptor.GetValue(anonymousObject);
                expando.Add(propertyDescriptor.Name, obj);
            }

            return (ExpandoObject)expando;
        }

        public static dynamic ToDynamic<T>(this T anonymousObject) where T : class
        {
            return (dynamic)anonymousObject.ToExpando();
        }

        public static ExpandoObject ToExpando(this Dictionary<string, string> dictionary)
        {
            IDictionary<string, object> expando = new ExpandoObject();
            foreach (var key in dictionary.Keys)
            {
                object obj = dictionary[key];
                expando.Add(key, obj);
            }

            return (ExpandoObject)expando;
        }

        public static dynamic ToDynamic(this Dictionary<string, string> dictionary)
        {
            return (dynamic)dictionary.ToExpando();
        }

        public static string[] AddItem(this string[] array, string item)
        {
            List<string> collection = array.ToList();
            if (array == null || array.Length == 0)
                collection.Add(item);
            else if (!array.Contains(item))
                collection.Add(item);

            return collection.ToArray();
        }

        public static string[] RemoveItem(this string[] array, string item)
        {
            List<string> collection = array.ToList();
            if (array == null || array.Length == 0 || !array.Contains(item))
                return array;

            collection.Remove(item);

            return collection.ToArray();
        }

        /// <summary>
        /// 提供可以直接使用 {0}, {1} 的方式，透過後面參數傳入；如：
        /// "Test {0} is {1}".FormatWith("apple", "orange");
        /// </summary>
        /// <param name="formatString">字串，內可用 {0}, {1} 替代</param>
        /// <param name="args">第一個參數代表{0}，第二個{1}，以此類推</param>
        /// <returns></returns>
        public static string FormatWith(this string formatString, params object[] args)
        {
            return args == null || args.Length == 0 ? formatString : string.Format(formatString, args);
        }

        // Convert the string to Pascal case.
        public static string ToPascalCase(this string target)
        {
            // If there are 0 or 1 characters, just return the string.
            if (target == null) return target;
            if (target.Length < 2) return target.ToUpper();

            // Split the string into words.
            string[] words = target.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            // Combine the words.
            string result = "";
            foreach (string word in words)
            {
                result +=
                    word.Substring(0, 1).ToUpper() +
                    word.Substring(1);
            }

            return result;
        }

        // Convert the string to camel case.
        public static string ToCamelCase(this string target)
        {
            // If there are 0 or 1 characters, just return the string.
            if (target == null || target.Length < 2)
                return target;

            // Split the string into words.
            string[] words = target.Split(
                new char[] { },
                StringSplitOptions.RemoveEmptyEntries);

            string result = string.Empty;
            if (words.Length == 1)
            {
                result = target.Substring(0, 1).ToLower();
                result += target.Substring(1);
            }
            else 
            {
            // Combine the words.
            result = words[0].ToLower();
            for (int i = 1; i < words.Length; i++)
            {
                result +=
                    words[i].Substring(0, 1).ToUpper() +
                    words[i].Substring(1);
            }
            }
            return result;
        }

        /// <summary>
        /// 提供字串在後面增加其他字串，並且透過 seperator 分隔
        /// 例如： 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="appendString"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
        public static string AppendSeperator(this string target, string appendString, string seperator = " ")
        {
            if (string.IsNullOrEmpty(target))
                return appendString;

            return target + seperator + appendString;
        }

        public static Dictionary<int, string> ToEnumDictionary<T>() where T : struct
        {
            return ((IEnumerable<T>)Enum
                .GetValues(typeof(T)))
                .ToDictionary(
                    item => Convert.ToInt32(item),
                    item => item.ToString());
        }

        public static Dictionary<int, string> ToEnumDictionary(Type enumType)
        {
            if (enumType.GetTypeInfo().IsEnum)
            {
                var result = new Dictionary<int, string>();
                foreach(var value in Enum.GetValues(enumType))
                {
                    result.Add((int)value, value.ToString());
                }
                return result;
            }

            return null;
        }

        public static IMappingExpression<TSource, TDestination> 
            IgnoreAllNonExisting<TSource, TDestination>(
                this IMappingExpression<TSource, TDestination> expression)
        {
            var sourceType = typeof(TSource);
            var destinationType = typeof(TDestination);
            var existingMaps = Mapper
                    .Configuration
                    .GetAllTypeMaps()
                    .First(x => 
                        x.SourceType.Equals(sourceType)
                        && x.DestinationType.Equals(destinationType)
                    );
            foreach (var property in existingMaps.GetUnmappedPropertyNames())
            {
                expression.ForMember(property, opt => opt.Ignore());
            }
            return expression;
        }

        /// <summary>
        /// 主要提供 TagHeper 使用，主要目的在於取出 Property 的 Dropdown Attribute
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string GetDropdownAttribute(this ModelExpression model)
        {
            // 預設依據 Code 名稱找 CodeService
            var code = model.ModelExplorer.Metadata.PropertyName;

            // 如果有定義 Dropdown Attribute ，則以 Dropdown Attribute 為主找 CodeService 
            var dropdown = model.Metadata.ContainerType.GetProperty(code)
                                .GetCustomAttributes()
                                .OfType<DropdownAttribute>().FirstOrDefault();

            if (dropdown != null)
                code = dropdown.CodeMap;
            return code;
        }
    }
}
