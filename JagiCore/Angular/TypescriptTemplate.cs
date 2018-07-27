using System;
using System.Collections.Generic;
using JagiCore.Helpers;

namespace JagiCore.Angular
{
    public class TypescriptTemplate
    {
        /// <summary>
        /// 除了 boolean 跟 number 之外，一律回傳 string type （DateTime 也是用 string type 表達） 
        /// </summary>
        public static string Generate(Type type, List<PropertyRule> properties)
        {
            var classTitle = CLASS_TITLE.FormatWith(type.Name);
            string strProperties = GetProperties(properties);

            return classTitle + CLASS_CONSTRUCTOR + strProperties + CLASS_ENDING;
        }

        private static string GetProperties(List<PropertyRule> properties)
        {
            string result = string.Empty;

            properties.ForEach(property => {
                result += CLASS_PROPERTY.FormatWith(property.Name, GetType(property.InputType));
            });

            return result.TrimEnd(new char[] { ',', '\n' }) + "\n";
        }

        private static string GetType(InputTag inputType)
        {
            if (inputType == InputTag.Checkbox)
                return "boolean";
            
            if (inputType == InputTag.InputNumber)
                return "number";

            return "string";
        }
        /// <summary>
        /// 因為有使用 FormatWith 因此 { 必須要用 {{ 替代，否則會被誤認
        /// </summary>
        private const string CLASS_TITLE = "export class {0} {{\n";
        private const string CLASS_CONSTRUCTOR = "  constructor(\n";
        private const string CLASS_PROPERTY = "     public {0}: {1},\n";
        private const string CLASS_ENDING = ") { }\n}";
    }
}