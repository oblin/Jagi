using JagiCore;
using JagiCore.Interfaces;
using JagiCore.Services;
using JagiCoreWeb.Controllers;
using JagiCoreWeb.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class NgTemplateControllerTest
    {
        [Fact]
        public void Get_Class_By_Controller()
        {
            var controller = new NgTemplateController();
            var getResult = controller.Get("NgTemplate"); // from JagiCoreWeb
            var jsonResult = Assert.IsType<JsonResult>(getResult);
            var template = jsonResult.Value as NgTemplate;

            Assert.Equal("NgTemplate", template.ClassName);
        }

        [Fact]
        public void Get_Class_By_JagiCore_Class()
        {
            var controller = new NgTemplateController();
            var getResult = controller.Get("JagiCore", "Result");   // from JagiCore
            var jsonResult = Assert.IsType<JsonResult>(getResult);
            var template = jsonResult.Value as NgTemplate;

            Assert.Equal("Result", template.ClassName);
        }

        [Fact]
        public void NotFound_On_Wrong_Class_Name()
        {
            var controller = new NgTemplateController();
            var getResult = controller.Get("JagiCore", "WronggResult");
            var result = Assert.IsType<NotFoundObjectResult>(getResult);

            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void NotFound_On_Wrong_Assembly_Name()
        {
            var controller = new NgTemplateController();
            var getResult = controller.Get("WrongJagiCore", "Result");
            var result = Assert.IsType<NotFoundObjectResult>(getResult);

            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public void Test_Class_Typescript()
        {
            var controller = new NgTemplateController();
            var getResult = controller.Get("NgTemplate"); // from JagiCoreWeb
            var jsonResult = Assert.IsType<JsonResult>(getResult);
            var template = jsonResult.Value as NgTemplate;

            var classes = template.ClassScript.Split('\n');
            Assert.Equal("export class NgTemplate {", classes[0]);
            Assert.Equal("  constructor(", classes[1]);
            Assert.Equal("     public ClassName: string,", classes[2]);
            Assert.Equal("     public ClassScript: string,", classes[3]);
            Assert.Equal("     public ClassHtml: string", classes[4]);
            Assert.Equal(") { }", classes[5]);
            Assert.Equal("}", classes[6]);
        }

        [Fact]
        public void Test_Class_Html()
        {
            var controller = new NgTemplateController();
            var getResult = controller.Get("NgTemplate"); // from JagiCoreWeb
            var jsonResult = Assert.IsType<JsonResult>(getResult);
            var template = jsonResult.Value as NgTemplate;

            var html = template.ClassHtml;
            Assert.True(html.Contains("[(ngModel)]=\"model.ClassName\""));
            Assert.True(html.Contains("[(ngModel)]=\"model.ClassScript\""));
            Assert.True(html.Contains("[(ngModel)]=\"model.ClassHtml\""));
        }
    }
}
