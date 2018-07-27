using System;
using System.Linq.Expressions;
using System.Reflection;
using JagiCore.Angular;

namespace JagiCore.Services
{
    public static class CodeServiceHelper
    {
        /// <summary>
        /// 透過 strong type 取出 code service 對應的 description。注意此屬性必須要宣告 DropdownAttribute 否則會出錯
        /// 使用方法：CodeServiceHelper.GetCodeValue<T, string>(p => p.OutSource, _codeService)
        /// 如果有需要對應到 ParentCode，則要在 DropdownAttribute 中，定義 ParentCodeMap & ParentFieldName
        /// </summary>
        /// <typeparam name="T">class type</typeparam>
        /// <typeparam name="TOut">設定 string 即可</typeparam>
        /// <param name="memberLamda">要有宣告 DropdownAttribute 的屬性</param>
        /// <param name="codeService"></param>
        /// <returns></returns>
        public static string GetCodeValue<T>(this T obj, Expression<Func<T, string>> memberLamda, CodeService codeService)
        {
            var expression = (MemberExpression)memberLamda.Body;
            if (expression == null)
                throw new NullReferenceException("無法使用的 expression action");

            var propInfo = (PropertyInfo)expression.Member;
            var attr = propInfo.GetCustomAttribute(typeof(DropdownAttribute), true) as DropdownAttribute;

            if (attr != null)
            {
                var code = attr.CodeMap;
                var value = propInfo.GetValue(obj);

                if (value == null || value.ToString() == string.Empty)
                    return string.Empty;

                var parentCode = attr.ParentCodeMap;
                var parentField = attr.ParentFieldName;
                if (!string.IsNullOrEmpty(parentField))
                {
                    // 如果有宣告對應的 Parent ItemType，則要傳入 Parent Code Value
                    var parentPropInfo = typeof(T).GetProperty(parentField);
                    var parentValue = parentPropInfo.GetValue(obj);
                    // 不判斷 parent code value != null 目的在於直接產生錯誤，不要有錯誤的設定
                    return codeService.GetDescription(code, value?.ToString(), parentValue.ToString())
                        .OnBoth(result => result.IsSuccess ? result.Value : string.Empty);
                }
                else
                {
                    return codeService.GetDescription(code, value?.ToString())
                        .OnBoth(result => result.IsSuccess ? result.Value : string.Empty);
                }
            }

            throw new ArgumentNullException("找不到對應的 DropdownAttribute");
        }
    }
}
