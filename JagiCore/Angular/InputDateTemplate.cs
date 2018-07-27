using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Angular
{
    public class InputDateTemplate : TemplateElement
    {
        public InputDateTemplate(TemplateElement next) : base(InputTag.Date, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            SetPropertyFields(property, layout);

            string pattern = @"([0-9]{2,4})\/([0-9]{1,2})\/([0-9]{1,2})";

            return HTML.FormatWith(
                templateVariable, fieldName, modelName, validationString,
                labelName, tooltip, formGroupWidth, labelWidth, controlWidth,
                placeholder, tooltipEnable, pattern, formGroupRequired);
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
        /// {11}: pattern: Date pattern
        /// {12}: form-group required class
        /// </summary>
        protected const string HTML =
            "<form-group [width]=\"{6}\" [controlVariable]=\"{0}\" [required]=\"{12}\">\n" +
            "	<label class=\"control-label col-sm-{7}\" for=\"{0}\">{4}</label>\n" +
            "	<div class=\"col-sm-{8}\">\n" +
            "		<div class=\"input-group\">\n" +
            "			<input id=\"{0}\" name=\"{1}\" class=\"form-control\" type=\"text\" {9}\n" +
            "				   #{0}=\"ngModel\" pattern=\"{11}\" {3}\n" +
            "				   [(ngModel)]=\"{2}.{1}\" (blur)=\"{2}.{1} = dateFormat({2}.{1})\" {5} {10} />\n" +
            "			<my-datepicker [dateModel]=\"{2}.{1}\" [controlVariable]=\"{0}\" (dateModelChange)=\"{2}.{1} = $event\" class=\"input-group-btn\"></my-datepicker>\n" +
            "		</div>\n" +
            "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" +
            "	</div>\n" +
            "</form-group>";
    }
}
