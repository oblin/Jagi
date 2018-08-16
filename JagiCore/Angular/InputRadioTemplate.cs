using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Angular
{
    public class InputRadioTemplate : TemplateElement
    {
        public InputRadioTemplate(TemplateElement next) : base(InputTag.Radio, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            if (property.RadioOptions.Count > 0)
            { 
                SetPropertyFields(property, layout);

                string radioOptions = CreateRadioOptions(property.RadioOptions);

                // Radio form-group 使用固定的 label 格式
                string previousHtml = PRE_HTML.FormatWith(templateVariable, labelName, formGroupWidth, 3, 9, formGroupRequired);

                string afterHtml = AFT_HTML.FormatWith(templateVariable);

                return previousHtml + radioOptions + afterHtml;
            }

            throw new ArgumentNullException("使用 Enum Type 但沒有 Radio Options");
        }

        private string CreateRadioOptions(List<Option> radioOptions)
        {
            var result = string.Empty;
            foreach(var item in radioOptions)
            {
                result += RADIO_HTML.FormatWith(templateVariable, fieldName, modelName, item.Value, item.Name);
            }

            return result;
        }

        /// <summary>
        /// {0}: #name template-driven vairable
        /// {1}: Model field name
        /// {2}: model name
        /// {3}: Option Value
        /// {4}: Option e (display name)
        /// </summary>
        protected const string RADIO_HTML =
            "		<div class=\"radio radio-inline\">\n" +
            "			<label>\n" +
            "				<input type=\"radio\" id=\"{0}\" name=\"{1}\" value=\"{3}\" #{0}=\"ngModel\"\n" +
            "					   [(ngModel)]=\"{2}.{1}\" />{4}\n" +
            "			</label>\n" +
            "		</div>\n";

        /// <summary>
        /// {0}: #name template-driven vairable
        /// {1}: Label Name (display name)
        /// {2}: form-group width
        /// {3}: label width
        /// {4}: control width
        /// {5}: form-group required class
        /// </summary>
        protected const string PRE_HTML =
            "<form-group [width]=\"{2}\" [controlVariable]=\"{0}\" [required]=\"{5}\">\n" +
            "	<label class=\"control-label col-sm-{3}\" for=\"{0}\">{1}</label>\n" +
            "	<div class=\"col-sm-{4}\">\n";

        /// <summary>
        /// {0}: #name template-driven vairable
        /// </summary>
        protected const string AFT_HTML =
            //    "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" + 移除 validate-span 因為改用 form-group 控制
            "	</div>\n" +
            "</form-group>";
    }
}
