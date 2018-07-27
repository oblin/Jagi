using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Angular
{
    public class SelectForTemplate : TemplateElement
    {
        public SelectForTemplate(TemplateElement next): base(InputTag.SelectFor, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            SetPropertyFields(property, layout);

            string code = property.CodeMap;
            string codeFor = property.CodeMapFor;
            string relativeField = property.CodeMapForField;

            return HTML.FormatWith(
                templateVariable, fieldName, modelName, validationString,
                labelName, code, formGroupWidth, labelWidth, controlWidth,
                codeFor, relativeField, formGroupRequired);
        }

        /// <summary>
        /// {0}: #name template-driven vairable
        /// {1}: Model field name
        /// {2}: model name
        /// {3}: validations (所有檢驗的項目都放在一起)
        /// {4}: Label Name (display name)
        /// {5}: Code: 代碼
        /// {6}: form-group width
        /// {7}: label width
        /// {8}: control width
        /// {9}: Code For: change for child code
        /// {10}: child model name: Same as Code For Name (if empty)
        /// {11}: form-group required class
        /// </summary>
        protected const string HTML =
            "<form-group [width]=\"{6}\" [controlVariable]=\"{0}\" [required]=\"{11}\">\n" +
            "	<label class=\"control-label col-sm-{7}\" for=\"{0}\">{4}</label>\n" +
            "	<div class=\"col-sm-{8}\">\n" +
            "		<select class=\"form-control\" name=\"{1}\" id=\"{0}\" {3}\n" +
            "				[(ngModel)]=\"{2}.{1}\" #{0}=\"ngModel\"\n" +
            "				(ngModelChange)=\"onDropdownChange($event, '{9}'); {2}.{10} = '';\"\n" +
            "				code-options [codes]=\"getCode('{5}')\"></select>\n" +
        //    "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" + 移除 validate-span 因為改用 form-group 控制
            "	</div>\n" +
            "</form-group>";
    }


    public class SelectTemplate : TemplateElement
    {
        public SelectTemplate(TemplateElement next): base(InputTag.Select, next) { }

        protected override string Execute(PropertyRule property, FormGroupLayout layout)
        {
            SetPropertyFields(property, layout);

            string code = property.CodeMap;

            string parent = string.IsNullOrEmpty(property.ParentFieldName) 
                ? string.Empty
                : $" ,{property.ParentFieldName.First().ToString().ToLower()}" ;  // 讓第一個字元小寫

            return HTML.FormatWith(
                templateVariable, fieldName, modelName, validationString,
                labelName, code, formGroupWidth, labelWidth, controlWidth,
                formGroupRequired);
        }

        /// <summary>
        /// {0}: #name template-driven vairable
        /// {1}: Model field name
        /// {2}: model name
        /// {3}: validations (所有檢驗的項目都放在一起)
        /// {4}: Label Name (display name)
        /// {5}: Code: 代碼
        /// {6}: form-group width
        /// {7}: label width
        /// {8}: control width
        /// {9}: form-group required class
        /// {10}: parent-code
        /// </summary>
        protected const string HTML =
            "<form-group [width]=\"{6}\" [controlVariable]=\"{0}\" [required]=\"{9}\">\n" +
            "	<label class=\"control-label col-sm-{7}\" for=\"{0}\">{4}</label>\n" +
            "	<div class=\"col-sm-{8}\">\n" +
            "		<select class=\"form-control\" name=\"{1}\" id=\"{0}\" required\n" +
            "				[(ngModel)]=\"{2}.{1}\" #{0}=\"ngModel\"\n" +
            "				code-options [codes]=\"getCode('{5}')\">\n" +
            "		</select>\n" +
            //     "		<validate-span [controlVariable]=\"{0}\"></validate-span>\n" + 移除 validate-span 因為改用 form-group 控制
            "	</div>\n" +
            "</form-group>";
    }
}
