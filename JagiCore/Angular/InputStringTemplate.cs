using JagiCore.Helpers;

namespace JagiCore.Angular
{
    public class InputNumberTemplate : InputStringTemplate
    {
        public InputNumberTemplate(TemplateElement next) : base(InputTag.InputNumber, next) 
        {
            inputType = "number";
        }
    }

    public class InputStringTemplate : TemplateElement
    {
        protected string inputType;
        public InputStringTemplate(TemplateElement next) : base(InputTag.InputString, next)
        {
            inputType = "text";
        }

        public InputStringTemplate(InputTag type, TemplateElement next) : base(type, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            SetPropertyFields(property, layout);

            return HTML.FormatWith(
                templateVariable, fieldName, modelName, validationString,
                labelName, tooltip, formGroupWidth, labelWidth, controlWidth,
                placeholder, tooltipEnable, inputType, formGroupRequired);
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
        /// {11}: input type: 預設是 text，但也可以設定為數字
        /// {12}: form-group required class
        /// </summary>
        protected const string HTML =
            "<form-group [width]=\"{6}\" [controlVariable]=\"{0}\" [required]=\"{12}\">\n" +
            "	<label class=\"control-label col-sm-{7}\" for=\"{0}\">{4}</label>\n" +
            "	<div class=\"col-sm-{8}\">\n" +
            "		<input type=\"{11}\" id=\"{0}\" name=\"{1}\" {9} class=\"form-control\"\n" +
            "			   #{0}=\"ngModel\" {3}\n" +
            "			   [(ngModel)]=\"{2}.{1}\" {5} {10} />\n" +
       //     "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" + 移除 validate-span 因為改用 form-group 控制
            "	</div>\n" +
            "</form-group>";
    }
}
