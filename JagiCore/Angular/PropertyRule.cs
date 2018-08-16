using System;
using System.Collections.Generic;

using JagiCore.Helpers;

namespace JagiCore.Angular
{
    public class PropertyRule : ValidationDefinitions
    {
        public InputTag InputType { get; set; }
        public string DisplayName { get; set; }
        public string Prompt { get; set; }
        public string TemplateVariable { get { return this.Name.ToCamelCase(); } }
        public string Tooltip { get; set; }
        public string CodeMap { get; set; }
        public string CodeMapFor { get; set; }
        public string CodeMapForField { get; set; }
        public string ParentFieldName { get; set; }
        public List<Option> RadioOptions { get; set; }

        //public PropertyRule(string name, InputTag inputType)
        //{
        //    this.Name = name;
        //    this.InputType = inputType;
        //}
    }

    public class Option
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }

    public abstract class ValidationDefinitions
    {
        public ValidationDefinitions()
        {
            Validations = new List<PropertyValidation>();
        }

        /// <summary>
        /// 欄位名稱
        /// </summary>
        public string Name { get; set; }
        public List<PropertyValidation> Validations { get; protected set; }
        public void AddValidation(ValidationType type, params object[] args)
        {
            int value = args != null && args.Length > 0 ? Convert.ToInt32(args[0]) : 0;
            switch (type)
            {
                case ValidationType.Required:
                    this.Validations.Add(RequiredValidation.FormatMessage(this.Name));
                    break;
                case ValidationType.MaxLength:
                    this.Validations.Add(MaxLengthValidation.FormatMessage(this.Name, value));
                    break;
                case ValidationType.MinLength:
                    this.Validations.Add(MinLengthValidation.FormatMessage(this.Name, value));
                    break;
                case ValidationType.MaxValue:
                    this.Validations.Add(MaxValueValidation.FormatMessage(this.Name, value));
                    break;
                case ValidationType.MinValue:
                    this.Validations.Add(MinValueValidation.FormatMessage(this.Name, value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private PropertyValidation RequiredValidation =
            new PropertyValidation
            {
                Type = ValidationType.Required,
                Message = "【{0}】必須要輸入"
            };

        private PropertyValidation MinLengthValidation =
            new PropertyValidation
            {
                Type = ValidationType.MinLength,
                Message = "【{0}】最小長度為：{1}"
            };

        private PropertyValidation MaxLengthValidation =
            new PropertyValidation
            {
                Type = ValidationType.MaxLength,
                Message = "【{0}】最大長度不可超過：{1}"
            };

        private PropertyValidation MinValueValidation =
            new PropertyValidation
            {
                Type = ValidationType.MinValue,
                Message = "【{0}】數值最小必須大於：{1}"
            };

        private PropertyValidation MaxValueValidation =
            new PropertyValidation
            {
                Type = ValidationType.MaxValue,
                Message = "【{0}】數值最大必須小於：{1}"
            };

        private PropertyValidation RangeValidation =
            new PropertyValidation
            {
                Type = ValidationType.Range,
                Message = "【{0}】數值必須要介於：{1} 跟 {2} 之間"
            };
    }

    public class PropertyValidation
    {
        public ValidationType Type { get; set; }
        public string Message { get; set; }
        public int Value { get; set; }

        public PropertyValidation FormatMessage(string name)
        {
            this.Message = string.Format(this.Message, name);
            return this;
        }

        public PropertyValidation FormatMessage(string name, int value1)
        {
            this.Value = value1;
            this.Message = string.Format(this.Message, name, value1);
            return this;
        }
    }

    public enum ValidationType
    {
        Required,
        MaxLength,
        MinLength,
        Range,
        MaxValue,
        MinValue
    }

    public enum InputTag
    {
        InputString,
        InputNumber,
        Date,
        Select,
        SelectFor,
        Textarea,
        Checkbox,
        Radio
    }
}