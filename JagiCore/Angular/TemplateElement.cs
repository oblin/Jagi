using System;
using System.Linq;
using JagiCore.Helpers;

namespace JagiCore.Angular
{
    public abstract class TemplateElement
    {
        private readonly TemplateElement _next;
        private readonly InputTag _type;

        public TemplateElement(InputTag type, TemplateElement next)
        {
            this._next = next;
            this._type = type;
        }

        public string Generate(PropertyRule property, FormGroupLayout layout = null)
        {
            layout = layout ?? new FormGroupLayout(4);

            if (property.InputType == _type)
            {
                return Execute(property, layout);
            }
            else 
            {
                if (_next == null)
                    throw new NullReferenceException($"沒有處理 Property: {property.Name} - {property.InputType} 的對應 Template ");
                else 
                    return _next.Generate(property, layout);
            }
        }

        /// <summary>
        /// 繼承的類別必須要實作說明如何進行設定
        /// </summary>
        /// <param name="property">說明欄位的屬性</param>
        /// <param name="layout">設定 Bootstrap 的 layout</param>
        /// <returns></returns>
        protected abstract string Execute(PropertyRule property, FormGroupLayout layout);

        #region 提供預設的 property & layout 的設定內容
        protected string templateVariable, fieldName, modelName, validationString, labelName;
        protected int labelWidth, controlWidth, formGroupWidth;
        protected string placeholder = string.Empty;
        protected string tooltip, tooltipEnable, formGroupRequired;

        protected virtual void SetPropertyFields(PropertyRule property, FormGroupLayout layout)
        {
            templateVariable = property.TemplateVariable;
            fieldName = property.Name;
            modelName = layout.ModelName;
            labelName = property.DisplayName;
            formGroupWidth = layout.FormGrid;
            labelWidth = layout.LabelGrid;
            controlWidth = layout.InputGrid;
            placeholder = string.Empty;
            formGroupRequired = string.Empty;

            string validationMessages;
            validationString = GetValidations(property, out validationMessages);
            SetTooltip(property, validationMessages);
        }

        private string GetValidations(PropertyRule property, out string validationMessages)
        {
            string result = string.Empty;
            string message = string.Empty;
            var validations = property.Validations;
            formGroupRequired = "false";
            if (validations.Any(v => v.Type == ValidationType.Required))
            {
                formGroupRequired = "true";
                result += "required";
            }

            validations.ForEach(validation => {
                if (_type == InputTag.InputNumber)
                {
                    if (validation.Type == ValidationType.MinValue){
                        result = result.AppendSeperator($"min=\"{validation.Value}\"");             
                        message = message.AppendSeperator(validation.Message);       
                    } 
                    if (validation.Type == ValidationType.MaxValue){
                        result = result.AppendSeperator($"max=\"{validation.Value}\"");
                        message = message.AppendSeperator(validation.Message);                    
                    } 
                }
                if (_type == InputTag.InputString)
                {
                    if (validation.Type == ValidationType.MinLength)
                    {
                        result = result.AppendSeperator($"minlength=\"{validation.Value}\"");
                        message = message.AppendSeperator(validation.Message);                    
                    }
                    if (validation.Type == ValidationType.MaxLength)
                    {
                        result = result.AppendSeperator($"maxlength=\"{validation.Value}\"");
                        message = message.AppendSeperator(validation.Message);                    
                    }
                }
            });

            validationMessages = message;
            return result;
        }

        private void SetTooltip(PropertyRule property, string validationMessages)
        {
            tooltip = tooltipEnable = string.Empty;
            if (string.IsNullOrEmpty(property.Tooltip) && string.IsNullOrEmpty(validationMessages))
                return;

            string message = string.IsNullOrEmpty(property.Tooltip) ? validationMessages : property.Tooltip;
            if (!string.IsNullOrEmpty(message))
            {
                tooltip = $"tooltip=\"{message}\"";
                tooltipEnable = $"[tooltipEnable]=\"{property.TemplateVariable}.invalid\"";
            }
        }
        #endregion
    }
}