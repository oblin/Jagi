using JagiCore;
using JagiCore.Interfaces;
using JagiCore.Services;
using JagiCoreWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class CodesControllerTest
    {
        [Fact]
        public void Test_Codes_Controller()
        {
            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.GetAll().Returns(CodeSample.FakeCodes());
            var service = CodeService.Create(repo);

            var controller = new CodesController(service, null);
            var getResult = controller.Get("County");
            var jsonResult = Assert.IsType<JsonResult>(getResult);
            var codes = jsonResult.Value as List<Code>;

            Assert.Equal(3, codes.Count);
            Assert.Equal("00 台北市", codes.First().Description);
        }

        [Fact]
        public void Test_Codes_Controller_Use_ParentCode()
        {
            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.GetAll().Returns(CodeSample.FakeCodes());
            var service = CodeService.Create(repo);

            var controller = new CodesController(service, null);
            var getResult = controller.Get("Hospital", "01");
            var jsonResult = Assert.IsType<JsonResult>(getResult);
            var codes = jsonResult.Value as List<Code>;

            Assert.Equal(4, codes.Count);
            Assert.Equal("0001 台大醫院", codes.First().Description);
        }

        [Fact]
        public void Test_Codes_Controllers_Use_Empty_ParentCode()
        {
            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.GetAll().Returns(CodeSample.FakeCodes());
            var service = CodeService.Create(repo);

            var controller = new CodesController(service, null);
            var getResult = controller.Get("Hospital", "");
            var jsonResult = Assert.IsType<NotFoundResult>(getResult);
        }

        [Fact]
        public void Test_Codes_Controllers_Get_By_Id()
        {
            Assembly assembly = typeof(CodeFile).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            config.Execute();

            var repo = Substitute.For<IRepository<CodeFile>>();
            var codeFile = new CodeFile
            {
                Id = 1,
                ItemType = "County"
            };
            repo.Find(Arg.Any<int>()).Returns(codeFile.ToResult());

            var controller = new CodesController(null, repo);
            var getResult = controller.GetById(1);

            var jsonResult = Assert.IsType<OkObjectResult>(getResult);
            var code = jsonResult.Value as CodeFile;

            Assert.Equal("County", code.ItemType);
        }

        [Fact]
        public void Get_All_Codes_Controllers()
        {
            Assembly assembly = typeof(CodeFile).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            config.Execute();

            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.GetAll().Returns(CodeSample.FakeCodes());
            var service = CodeService.Create(repo);

            var controller = new CodesController(service, repo);
            var getResult = controller.GetCodeFiles();
            var jsonResult = Assert.IsType<OkObjectResult>(getResult);
            var codes = jsonResult.Value as List<CodeFile>;

            Assert.Equal(2, codes.Count);
            Assert.Equal("County", codes.First().ItemType);
            Assert.Equal("Hospital", codes.Last().ItemType);
        }
        
        [Fact]
        public void Can_Save_Code()
        {
            Assembly assembly = typeof(CodeFile).GetTypeInfo().Assembly;
            var config = new MappingBaseConfig(assembly);
            config.Execute();

            var code = new CodeFile();

            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.Find(Arg.Any<int>()).Returns(code.ToResult());

            var controller = new CodesController(null, repo);
            var result = controller.SaveCodeFile(code);

            repo.Received().Save();

            code.Id = 1;
            result = controller.SaveCodeFile(code);
            repo.Received().Update(code);
        }

        [Fact]
        public void Can_Delete_CodeFile()
        {
            var repo = Substitute.For<IRepository<CodeFile>>();
            repo.Find(Arg.Any<int>()).Returns(Result.Ok(new CodeFile()));

            var controller = new CodesController(null, repo);
            var result = controller.DeleteCodeFile(1);

            repo.Received().Remove(1);
            var jsonResult = Assert.IsType<OkResult>(result);
        }
    }
}
