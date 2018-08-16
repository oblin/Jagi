using JagiCore.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace JagiCore.Angular
{
    public class ModelParser
    {
        private const string buttonsHtml =
            "<p>\n" +
            "	<button class=\"btn btn-primary\" type=\"submit\"\n" +
            "			[disabled]=\"form.pristine || form.invalid\">\n" +
            "		存檔\n" +
            "	</button>\n" +
            "	<button class=\"btn btn-warning\" (click)=\"cancel(form)\" type=\"button\">\n" +
            "		取消\n" +
            "	</button>\n" +
            "</p>\n";
        private const string formHtml = "<form #form=\"ngForm\" (submit)=\"submitForm(form)\" class=\"form-horizontal\" novalidate>\n";
        private const string rowHtml = "<div class=\"row\">";

        public static string CreateForm(Type type, FormGroupLayout layout = null)
        {
            var handler = new InputDateNgxTemplate(
                new SelectTemplate(
                    new SelectForTemplate(
                        new InputRadioTemplate(
                            new InputCheckTemplate(
                                new InputNumberTemplate(
                                    new InputStringTemplate(null)))))));

            layout = layout ?? new FormGroupLayout(4);

            string result = BuildupFormWithRow(type, layout, handler);

            return formHtml + buttonsHtml + result + "\n</form>";
        }

        public static string CreateTypescriptClass(Type type)
        {
            var properties = CreateProperties(type);
            return TypescriptTemplate.Generate(type, properties);
        }

        public static List<PropertyRule> CreateProperties(Type type)
        {
            var result = new List<PropertyRule>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var property in properties)
            {
                if (IsHidden(property))
                    continue;

                InputTag inputType = GetPropertyInputType(property);
                var propertyRule = new PropertyRule();
                propertyRule.Name = property.Name;
                propertyRule.InputType = inputType;

                if (IsRequired(property))
                    propertyRule.AddValidation(ValidationType.Required, null);

                int arg;
                if (MaxStringLength(property, out arg))
                    propertyRule.AddValidation(ValidationType.MaxLength, new object[]{ arg });

                if (MinStringLength(property, out arg))
                    propertyRule.AddValidation(ValidationType.MinLength, new object[]{ arg });

                if (MinValue(property, out arg))
                    propertyRule.AddValidation(ValidationType.MinValue, new object[] { arg });

                if (MaxValue(property, out arg))
                    propertyRule.AddValidation(ValidationType.MaxValue, new object[] { arg });

                SetPropertyDisplayFields(property, propertyRule);
                SetDropdownCode(property, propertyRule);
                SetRadioButton(property, propertyRule);

                result.Add(propertyRule);
            }

            return result;
        }

        private static bool IsHidden(PropertyInfo property)
        {
            var hidden = property.GetCustomAttributes().OfType<HiddenInputAttribute>().FirstOrDefault();
            return hidden != null;
        }

        private static void SetRadioButton(PropertyInfo property, PropertyRule propertyRule)
        {
            if (property.PropertyType.GetTypeInfo().IsEnum)
            {
                var dict = TypeHelper.ToEnumDictionary(property.PropertyType);
                propertyRule.RadioOptions = new List<Option>();
                foreach(var item in dict)
                {
                    propertyRule.RadioOptions.Add(new Option { Name = item.Value, Value = item.Key });
                }
            }
        }

        private static void SetDropdownCode(PropertyInfo property, PropertyRule propertyRule)
        {
            var dropdown = property.GetCustomAttributes()
                                .OfType<DropdownAttribute>().FirstOrDefault();
            if (dropdown != null)
                propertyRule.CodeMap = dropdown.CodeMap;

            var dropdownFor = property.GetCustomAttributes()
                                .OfType<DropdownForAttribute>().FirstOrDefault();
            if (dropdownFor != null)
            {
                propertyRule.CodeMap = dropdownFor.CodeMap;
                propertyRule.CodeMapFor = dropdownFor.CodeMapFor;
                propertyRule.CodeMapForField = dropdownFor.ChildFieldName ?? dropdownFor.CodeMapFor;
            }
        }

        private static void SetPropertyDisplayFields(PropertyInfo property, PropertyRule rule)
        {
            var displayInfo = property.GetCustomAttributes().OfType<DisplayAttribute>().FirstOrDefault();
            rule.DisplayName = property.Name;
            rule.Tooltip = string.Empty;
            rule.Prompt = string.Empty;
            if (displayInfo != null )
            {
                rule.DisplayName = string.IsNullOrEmpty(displayInfo.Name) ? property.Name : displayInfo.Name;
                rule.Tooltip = displayInfo.Description;
                rule.Prompt = displayInfo.Prompt;
            }
        }

        public static void Generate()
        {
            var template = new InputStringTemplate(null);

        }

        private static bool MinValue(PropertyInfo property, out int arg)
        {
            var minValidations = property.GetCustomAttributes().OfType<RangeAttribute>().FirstOrDefault();

            arg = minValidations == null ? 0 : Convert.ToInt32(minValidations.Minimum);

            return minValidations != null;
        }

        private static bool MaxValue(PropertyInfo property, out int arg)
        {
            var minValidations = property.GetCustomAttributes().OfType<RangeAttribute>().FirstOrDefault();

            arg = minValidations == null ? 0 : Convert.ToInt32(minValidations.Maximum);

            return minValidations != null;
        }

        private static bool MaxStringLength(PropertyInfo property, out int arg)
        {
            var lengthValidations = property.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();

            arg = lengthValidations == null ? 0 : lengthValidations.MaximumLength;

            return lengthValidations != null;
        }

        private static bool MinStringLength(PropertyInfo property, out int arg)
        {
            var lengthValidations = property.GetCustomAttributes().OfType<StringLengthAttribute>().FirstOrDefault();

            arg = lengthValidations == null ? 0 :  lengthValidations.MinimumLength;

            return lengthValidations != null && arg > 0;
        }

        private static bool IsRequired(PropertyInfo property)
        {
            bool requiredAttr = property.GetCustomAttributes().OfType<RequiredAttribute>().FirstOrDefault() != null;
            bool typeRequired = 
                property.PropertyType == typeof(DateTime) ||
                property.PropertyType.GetTypeInfo().IsPrimitive;
            return requiredAttr || typeRequired;
        }

        private static InputTag GetPropertyInputType(PropertyInfo property)
        {
            if (property.PropertyType == typeof(DateTime) ||
                property.PropertyType == typeof(DateTime?))
                return InputTag.Date;

            if (IsDropdown(property))
                return InputTag.Select;

            if (IsDropdownFor(property))
                return InputTag.SelectFor;

            if (property.PropertyType == typeof(bool))
                return InputTag.Checkbox;

            if (property.PropertyType == typeof(int) ||
                property.PropertyType == typeof(float) ||
                property.PropertyType == typeof(double) ||
                property.PropertyType == typeof(decimal))
                return InputTag.InputNumber;

            if (property.PropertyType.GetTypeInfo().IsEnum)
                return InputTag.Radio;

            return InputTag.InputString;
        }

        private static bool IsDropdown(PropertyInfo property)
        {
            return property.GetCustomAttributes()
                    .OfType<DropdownAttribute>().FirstOrDefault()
                        != null;
        }

        private static bool IsDropdownFor(PropertyInfo property)
        {
            return property.GetCustomAttributes()
                    .OfType<DropdownForAttribute>().FirstOrDefault()
                        != null;
        }

        private static string BuildupFormWithRow(Type type, FormGroupLayout layout, TemplateElement handler)
        {
            string result = string.Empty;

            int counter = 0;
            int formGroupInRow = 12 / layout.FormGrid;
            foreach (var property in CreateProperties(type))
            {
                if (counter == 0)
                {
                    result = result.AppendSeperator(rowHtml, "\n");
                }

                result = result.AppendSeperator(handler.Generate(property, layout), "\n");

                counter += 1;

                if (counter == formGroupInRow)
                {
                    result = result.AppendSeperator("</div>", "\n");
                }

                if (counter >= formGroupInRow)
                    counter = 0;
            }

            if (counter < formGroupInRow && counter > 0)
            {
                result = result.AppendSeperator("</div>", "\n");
            }

            return result;
        }
    }
}