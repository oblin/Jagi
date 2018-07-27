using System;
using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace JagiCore.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// 將物件轉換成 Json 表達式字串
        /// ex: {"Number":11,"Text":"Sample Text","IsChinese":false,"StartDate":"0001-01-01T00:00:00","EndDate":null,"NullableInt":null}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">任意物件</param>
        /// <param name="includeNull">是否將 null 轉入預設 true，如果不要將 null 轉入，則會沒有 null 值的屬性</param>
        /// <returns></returns>
        public static string ToJsonString(this object target, bool includeNull = false)
        {
            return ExecuteJsonSerialize(target, includeNull, null);
        }

        /// <summary>
        /// 將物件轉換成 Json 表達式字串，包含 Null value 的屬性，這是 target.ToJsonString(true) 的簡寫
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string AllToJsonString(this object target)
        {
            return target.ToJsonString(true);
        }

        /// <summary>
        /// 將物件轉換成 Json 表達式字串，首字母改為小寫
        /// ex: {"number":11,"text":"Sample Text","isChinese":false,"startDate":"0001-01-01T00:00:00","endDate":null,"nullableInt":null}
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">任意物件</param>
        /// <param name="includeNull">是否將 null 轉入預設 true，如果不要將 null 轉入，則會沒有 null 值的屬性</param>
        /// <returns></returns>
        public static string ToCamelJsonString(this object target, bool includeNull = false)
        {
            return ExecuteJsonSerialize(target, includeNull, new CamelCasePropertyNamesContractResolver());
        }

        /// <summary>
        /// 轉換 Sample.Text 物件表達式，變成 "prefix.Text" 字串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="target"></param>
        /// <param name="property"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string ToPrefixPropertyString<T, TValue>(this T target, Expression<Func<T, TValue>> property, string prefix)
        {
            var expression = (MemberExpression)property.Body;
            if (expression == null)
                throw new NullReferenceException("無法使用的 expression action");

            var name = expression.Member.Name;
            return prefix + "." + name;
        }

        /// <summary>
        /// 將 json string 轉換成指定的 T type object
        /// ex: {"Id":1,"Text":"Test"} => Class.Id, Class.Text
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T JsonStringToObject<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }

        /// <summary>
        /// 將 json string 轉換成 dynamic 可以直接存取其 property
        /// ex: {"Id":1,"Text":"Test"} => dynamic.Id, dynamic.Text
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static dynamic JsonStringToObject(this string jsonString)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(jsonString);
        }

        private static string ExecuteJsonSerialize(object target, bool includeNull, CamelCasePropertyNamesContractResolver camelCasting)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new StringEnumConverter() },
                NullValueHandling = includeNull ? NullValueHandling.Include : NullValueHandling.Ignore
            };

            settings.ContractResolver = camelCasting;

            return JsonConvert.SerializeObject(target, settings);
        }
    }
}
