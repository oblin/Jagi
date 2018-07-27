using JagiCore.Helpers;

namespace JagiCore.Angular
{
    public class DateTemplate : TemplateElement
    {
        public DateTemplate(TemplateElement next) : base(InputTag.InputString, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            SetPropertyFields(property, layout);

            return InputStringHtml.FormatWith(
                templateVariable, fieldName, modelName, validationString,
                labelName, tooltip, formGroupWidth, labelWidth, controlWidth, 
                placeholder, tooltipEnable);
        }

        /// <summary>
        /// {0}: #name template-driven vairable
        /// {1}: Model field name
        /// {2}: model name
        /// {3}: validations (所有檢驗的項目都放在一起)
        /// {4}: Label Name (display name)
        /// {5}: Tooltip: validation message (所有錯誤訊息都放在一起）
        /// {6}: form-group width
        /// {7}: label width
        /// {8}: control width
        /// {9}: placeholder="...."
        /// {10}: Tooltip Enable: 設定何時要啟動 tooltip
        /// </summary>
        public const string InputStringHtml =
            "<div class=\"form-group col-sm-{6} has-feedback required\" [ngClass]=\"{0}.dirty ? ({0}.valid ? 'has-success' : 'has-error') : ({0}.valid ? '' : 'has-warning')\">\n" +
            "	<label class=\"control-label col-sm-{7}\" for=\"{0}\">{4}</label>\n" +
            "	<div class=\"col-sm-{8}\">\n" +
            "		<input type=\"number\" id=\"{0}\" name=\"{1}\" {9} class=\"form-control\"\n" +
            "			   #{0}=\"ngModel\" {3}\n" +
            "			   [(ngModel)]=\"{2}.{1}\" {5} {10}\n" +
            "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" +
            "	</div>\n" +
            "</div>";
    }
}
