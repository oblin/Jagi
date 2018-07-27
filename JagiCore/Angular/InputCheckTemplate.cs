using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Angular
{
    public class InputCheckTemplate : TemplateElement
    {
        public InputCheckTemplate(TemplateElement next): base(InputTag.Checkbox, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            SetPropertyFields(property, layout);

            return HTML.FormatWith(
                templateVariable, fieldName, modelName, validationString,
                labelName, formGroupWidth, formGroupRequired);
        }

        /// <summary>
        /// {0}: #name template-driven vairable
        /// {1}: Model field name
        /// {2}: model name
        /// {3}: validations (所有檢驗的項目都放在一起)
        /// {4}: Label Name (display name)
        /// {5}: form-group width
        /// {6}: form-group required class
        /// </summary>
        protected const string HTML =
            "<form-group [width]=\"{5}\" [controlVariable]=\"{0}\" [required]=\"{6}\">\n" +
            "	<label>\n" +
            "		<input type=\"checkbox\" name=\"{1}\" {3} #{0}=\"ngModel\"\n" +
            "			   [(ngModel)]=\"{2}.{1}\" /> &nbsp; {4}\n" +
        //    "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" + 移除 validate-span 因為改用 form-group 控制
            "	</label>\n" +
            "</form-group>";
    }
}
