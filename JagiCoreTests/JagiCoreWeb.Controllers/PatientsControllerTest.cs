using JagiCore;
using JagiCore.Interfaces;
using JagiCoreWeb.Controllers;
using JagiCoreWeb.Models;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests.JagiCoreWeb.Controllers
{
    public class PatientsControllerTest
    {
        [Fact]
        public void Can_Get_Patient_By_id()
        {
            var repo = Substitute.For<IRepository<Patient>>();
            repo.Find(Arg.Any<int>()).Returns(Result.Ok(new Patient { Id = 1, Name = "Tester" }));
            var controller = new PatientsController(repo);

            var result = controller.Get(1);
            var jsonResult = Assert.IsType<OkObjectResult>(result);
            var patient = jsonResult.Value as Patient;

            Assert.Equal("Tester", patient.Name);
        }
    }
}
