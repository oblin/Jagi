using System;
using System.Linq;
using JagiCore.Angular;
using Xunit;

namespace JagiCoreTests
{
    public class HtmlGenerateTest
    {
        [Fact]
        public void Cannot_Generate_String_Input_HTML_Without_Template()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "Id");
            Assert.Equal(InputTag.InputNumber, propertyRule.InputType);

            var templateHandler = new InputStringTemplate(null);

            Assert.Throws<NullReferenceException>(() => templateHandler.Generate(propertyRule));

            templateHandler = new InputNumberTemplate(new InputStringTemplate(null));
            propertyRule = properties.SingleOrDefault(p => p.Name == "Date");

            Assert.Throws<NullReferenceException>(() => templateHandler.Generate(propertyRule));

            var dateTemplateHandler = new InputDateTemplate(new InputNumberTemplate(new InputStringTemplate(null)));
            propertyRule = properties.SingleOrDefault(p => p.Name == "Date");
            var html = dateTemplateHandler.Generate(propertyRule);
            Assert.True(html.Length > 1);
        }

        [Fact]
        public void Can_Generate_String_Input_HTML()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "FirstName");
            Assert.Equal(InputTag.InputString, propertyRule.InputType);

            var templateHandler = new InputStringTemplate(null);

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n'); 
            Assert.Equal(8, lineOfHtml.Length);
        }

        [Fact]
        public void Can_Generate_String_Input_HTML_From_Chains()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "FirstName");

            var templateHandler = new InputNumberTemplate(new InputStringTemplate(null));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n'); 
            Assert.Equal(8, lineOfHtml.Length);
            Assert.Equal("		<input type=\"text\" id=\"firstName\" name=\"FirstName\"  class=\"form-control\"",
                lineOfHtml[3]);
        }

        [Fact]
        public void Test_Input_String_Html_Lines()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "FirstName");

            var templateHandler = new InputStringTemplate(null);

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n'); 

            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"firstName\" [required]=\"true\">",
                lineOfHtml[0]);
            Assert.Equal("	<label class=\"control-label col-sm-6\" for=\"firstName\">名字</label>",
                lineOfHtml[1]);
            Assert.Equal("	<div class=\"col-sm-6\">",
                lineOfHtml[2]);
            Assert.Equal("		<input type=\"text\" id=\"firstName\" name=\"FirstName\"  class=\"form-control\"",
                lineOfHtml[3]);
            // Assert.Equal("			   #firstName=\"ngModel\" ",
            //     lineOfHtml[4]);
            Assert.Equal("			   [(ngModel)]=\"model.FirstName\" tooltip=\"此為必須輸入的欄位，且不可以超過30個字\" [tooltipEnable]=\"firstName.invalid\" />",
                lineOfHtml[5]);
            // 4-19 移除 validate-span
            //Assert.Equal("		<validate-span [controlVariable]=\"firstName\"></validate-span>",
            //    lineOfHtml[6]);
        }

        [Fact]
        public void Can_Generate_String_Input_HTML_Validations_MaxLength()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "FirstName");

            var templateHandler = new InputNumberTemplate(new InputStringTemplate(null));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n'); 
            Assert.Equal("			   #firstName=\"ngModel\" required maxlength=\"30\"",
                lineOfHtml[4]);
        }

        [Fact]
        public void Can_Generate_String_Input_HTML_Validations_NotRequired_Min_MaxLength()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "LastName");

            var templateHandler = new InputNumberTemplate(new InputStringTemplate(null));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"lastName\" [required]=\"false\">",
                lineOfHtml[0]);
            Assert.Equal("			   #lastName=\"ngModel\" maxlength=\"10\" minlength=\"5\"",
                lineOfHtml[4]);
        }

        [Fact]
        public void Can_Generate_String_Input_HTML_Specified_Model_Form_Layout()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "LastName");

            var templateHandler = new InputNumberTemplate(new InputStringTemplate(null));

            string html = templateHandler.Generate(propertyRule, new FormGroupLayout("testModel", 6));
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal("<form-group [width]=\"6\" [controlVariable]=\"lastName\" [required]=\"false\">",
                lineOfHtml[0]);
            Assert.True(lineOfHtml[5].Contains("testModel.LastName"));
        }

        [Fact]
        public void Can_Generate_Number_Input_HTML_Validations()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "Id");

            var templateHandler = new InputNumberTemplate(new InputStringTemplate(null));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n'); 
            Assert.Equal("			   #id=\"ngModel\" required min=\"10\" max=\"20\"",
                lineOfHtml[4]);
        }

        [Fact]
        public void Can_Generate_Number_Input_HTML_From_Chains()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "Id");

            var templateHandler = new InputNumberTemplate(new InputStringTemplate(null));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n'); 
            Assert.Equal(8, lineOfHtml.Length);
            Assert.Equal("		<input type=\"number\" id=\"id\" name=\"Id\"  class=\"form-control\"",
                lineOfHtml[3]);
        }

        [Fact]
        public void Can_Generate_Date_Input_HTML_From_Chains()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.SingleOrDefault(p => p.Name == "Date");

            var templateHandler = new InputDateTemplate(
                new InputNumberTemplate(new InputStringTemplate(null)));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal(12, lineOfHtml.Length);
        }

        [Fact]
        public void Test_Input_Date_Html_Lines()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "Date");

            var templateHandler = new InputDateTemplate(
                new InputNumberTemplate(new InputStringTemplate(null)));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');

            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"date\" [required]=\"true\">",
                lineOfHtml[0]);
            Assert.Equal("	<label class=\"control-label col-sm-6\" for=\"date\">Date</label>",
                lineOfHtml[1]);
            Assert.Equal("	<div class=\"col-sm-6\">",
                lineOfHtml[2]);
            Assert.Equal("		<div class=\"input-group\">",
                lineOfHtml[3]);
            Assert.Equal("			<input id=\"date\" name=\"Date\" class=\"form-control\" type=\"text\" ",
                lineOfHtml[4]);
            Assert.True(lineOfHtml[5].Contains("#date=\"ngModel\" pattern="));
            Assert.True(lineOfHtml[5].Contains("required"));
            Assert.Equal("				   [(ngModel)]=\"model.Date\" (blur)=\"model.Date = dateFormat(model.Date)\"   />",
                lineOfHtml[6]);
            Assert.Equal("			<my-datepicker [dateModel]=\"model.Date\" [controlVariable]=\"date\" (dateModelChange)=\"model.Date = $event\" class=\"input-group-btn\"></my-datepicker>",
                lineOfHtml[7]);
            Assert.Equal("		<validate-span [controlVariable]=\"date\"></validate-span>",
                lineOfHtml[9]);
        }

        [Fact]
        public void Test_Select_Html()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "Code");

            var templateHandler = new InputDateTemplate(
                new SelectForTemplate(
                    new InputStringTemplate(null)));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal(9, lineOfHtml.Length);
        }

        [Fact]
        public void Test_Select_Html_Lines()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "ChildCode");

            var templateHandler = new InputDateTemplate(
                new SelectTemplate(
                    new InputNumberTemplate(new InputStringTemplate(null))));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"childCode\" [required]=\"false\">",
                lineOfHtml[0]);
            Assert.Equal("	<label class=\"control-label col-sm-6\" for=\"childCode\">ChildCode</label>",
                lineOfHtml[1]);
            Assert.Equal("	<div class=\"col-sm-6\">",
                lineOfHtml[2]);
            Assert.Equal("		<select class=\"form-control\" name=\"ChildCode\" id=\"childCode\" required",
                lineOfHtml[3]);
            Assert.Equal("				[(ngModel)]=\"model.ChildCode\" #childCode=\"ngModel\"",
                lineOfHtml[4]);
            Assert.Equal("				code-options [codes]=\"getCode('Hospital')\">",
                lineOfHtml[5]);
            Assert.Equal("		</select>",
                lineOfHtml[6]);
            // 4-19 移除 validate-span
            //Assert.Equal("		<validate-span [controlVariable]=\"childCode\"></validate-span>",
            //    lineOfHtml[7]);
        }

        [Fact]
        public void Test_Only_One_ChildCode_Any_Properties_Count()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            Assert.Equal(6, properties.Count);

            var propertyRule = properties.Where(p => p.Name == "ChildCode");

            Assert.Equal(1, propertyRule.Count());
        }

        [Fact]
        public void Test_SelectFor_Html()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "ChildCode");

            var templateHandler = new InputDateTemplate(
                new SelectForTemplate(
                    new SelectTemplate(
                        new InputNumberTemplate(new InputStringTemplate(null)))));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal(9, lineOfHtml.Length); // 4-19 少一個 validate-span 
        }

        [Fact]
        public void Test_Select_For_Html_Lines()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass));

            var propertyRule = properties.FirstOrDefault(p => p.Name == "Code");

            var templateHandler = new InputDateTemplate(
                new SelectTemplate(
                    new SelectForTemplate(
                        new InputNumberTemplate(
                            new InputStringTemplate(null)))));

            string html = templateHandler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');
            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"code\" [required]=\"true\">",
                lineOfHtml[0]);
            Assert.Equal("	<label class=\"control-label col-sm-6\" for=\"code\">Code</label>",
                lineOfHtml[1]);
            Assert.Equal("	<div class=\"col-sm-6\">",
                lineOfHtml[2]);
            Assert.Equal("		<select class=\"form-control\" name=\"Code\" id=\"code\" required",
                lineOfHtml[3]);
            Assert.Equal("				[(ngModel)]=\"model.Code\" #code=\"ngModel\"",
                lineOfHtml[4]);
            Assert.Equal("				(ngModelChange)=\"onDropdownChange($event, 'Hospital'); model.ChildCode = '';\"", 
                lineOfHtml[5]);
            Assert.Equal("				code-options [codes]=\"getCode('County')\"></select>",
                lineOfHtml[6]);
            // 4-19 移除 validate-span
            //Assert.Equal("		<validate-span [controlVariable]=\"code\"></validate-span>",
            //    lineOfHtml[7]);
        }

        [Fact]
        public void Test_Input_Checkbox_Html()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass2));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Check");
            var handler = new InputCheckTemplate(new InputStringTemplate(null));

            string html = handler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');

            Assert.Equal(6, lineOfHtml.Length);
        }

        [Fact]
        public void Test_Input_Checkbox_Html_Lines()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass2));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Check");
            var handler = new InputCheckTemplate(new InputStringTemplate(null));

            string html = handler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');

            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"check\" [required]=\"true\">",
                lineOfHtml[0]);
            Assert.Equal("		<input type=\"checkbox\" name=\"Check\" required #check=\"ngModel\"",
                lineOfHtml[2]);
            Assert.Equal("			   [(ngModel)]=\"model.Check\" /> &nbsp; Check",
                lineOfHtml[3]);
            // 4-19 移除 validate-span
            //Assert.Equal("		<validate-span [controlVariable]=\"check\"></validate-span>",
            //    lineOfHtml[4]);
        }

        [Fact]
        public void Test_RadioButton_Html()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass2));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Radio");
            var handler = new InputRadioTemplate(new InputStringTemplate(null));

            string html = handler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');

            Assert.Equal(23, lineOfHtml.Length);
        }

        [Fact]
        public void Test_RadioButton_Html_Lines()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass2));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "Radio");
            var handler = new InputRadioTemplate(new InputStringTemplate(null));

            string html = handler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');

            Assert.Equal("<form-group [width]=\"4\" [controlVariable]=\"radio\" [required]=\"false\">",
                lineOfHtml[0]);
            Assert.Equal("	<label class=\"control-label col-sm-3\" for=\"radio\">Radio</label>",
                lineOfHtml[1]);
            Assert.Equal("	<div class=\"col-sm-9\">",
                lineOfHtml[2]);
            Assert.Equal("		<div class=\"radio radio-inline\">",
                lineOfHtml[3]);
            Assert.Equal("				<input type=\"radio\" id=\"radio\" name=\"Radio\" value=\"0\" #radio=\"ngModel\"",
                lineOfHtml[5]);
            Assert.Equal("					   [(ngModel)]=\"model.Radio\" />Option1",
                lineOfHtml[6]);
            Assert.Equal("			</label>",
                lineOfHtml[7]);
            Assert.Equal("				<input type=\"radio\" id=\"radio\" name=\"Radio\" value=\"1\" #radio=\"ngModel\"",
                lineOfHtml[11]);
            Assert.Equal("					   [(ngModel)]=\"model.Radio\" />Option2",
                lineOfHtml[12]);
            Assert.Equal("				<input type=\"radio\" id=\"radio\" name=\"Radio\" value=\"2\" #radio=\"ngModel\"",
                lineOfHtml[17]);
            Assert.Equal("					   [(ngModel)]=\"model.Radio\" />Option3",
                lineOfHtml[18]);
        }

        [Fact]
        public void Test_RadioButton_Html_Prefer_Way()
        {
            var properties = ModelParser.CreateProperties(typeof(VectorClass2));
            var propertyRule = properties.FirstOrDefault(p => p.Name == "PreferRadioStyle");
            var handler = new InputRadioTemplate(new InputStringTemplate(null));

            string html = handler.Generate(propertyRule);
            string[] lineOfHtml = html.Split('\n');

            Assert.Equal(23, lineOfHtml.Length);

            Assert.Equal("				<input type=\"radio\" id=\"preferRadioStyle\" name=\"PreferRadioStyle\" value=\"1\" #preferRadioStyle=\"ngModel\"",
                lineOfHtml[5]);
            Assert.Equal("					   [(ngModel)]=\"model.PreferRadioStyle\" />Option1",
                lineOfHtml[6]);
            Assert.Equal("				<input type=\"radio\" id=\"preferRadioStyle\" name=\"PreferRadioStyle\" value=\"2\" #preferRadioStyle=\"ngModel\"",
                lineOfHtml[11]);
            Assert.Equal("					   [(ngModel)]=\"model.PreferRadioStyle\" />Option2",
                lineOfHtml[12]);
            Assert.Equal("				<input type=\"radio\" id=\"preferRadioStyle\" name=\"PreferRadioStyle\" value=\"3\" #preferRadioStyle=\"ngModel\"",
                lineOfHtml[17]);
        }

        [Fact]
        public void Verify_Html_Generation()
        {
            var html = ModelParser.CreateForm(typeof(VectorClass));

            Assert.True(html.Contains("<div class=\"row\">"));
            Assert.True(html.Contains("				(ngModelChange)=\"onDropdownChange($event, 'Hospital'); model.ChildCode = '';\""));
            Assert.True(html.Contains("				[(ngModel)]=\"model.ChildCode\" #childCode=\"ngModel\""));
            Assert.True(html.Contains("#date=\"ngModel\" bsDatepicker #dbDate=\"bsDatepicker\" required"));
            Assert.True(html.Contains("			   [(ngModel)]=\"model.FirstName\" tooltip=\"此為必須輸入的欄位，且不可以超過30個字\" [tooltipEnable]=\"firstName.invalid\""));
            Assert.True(html.Contains("		<input type=\"text\" id=\"firstName\" name=\"FirstName\"  class=\"form-control\""));
            Assert.True(html.Contains("<form-group [width]=\"4\" [controlVariable]=\"lastName\" [required]=\"false\">"));
            Assert.True(html.Contains("	<label class=\"control-label col-sm-6\" for=\"childCode\">ChildCode</label>"));
        }
    }
}