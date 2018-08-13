using JagiCore.Admin.Tests.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JagiCore.Admin.Tests
{
    [TestClass]
    public class TestClinicService
    {
        [TestMethod]
        public void Test_Compatiable_With_ERS_Hope()
        {
            var setup = new AdminContextSetup();
            var provider = setup.CertProvider;

            byte[] bytes = new byte[] { 168, 9, 206, 134, 178, 210, 213, 154, 82, 223, 21, 193, 208, 224, 222, 84, 148, 110, 11, 177, 35, 212, 33, 6, 40, 227, 146, 159, 55, 141, 21, 251, 254, 60, 127, 38, 2, 204, 207, 171, 141, 129, 173, 125, 219, 138, 104, 143, 35, 202, 213, 74, 96, 212, 5, 204, 13, 178, 29, 141, 68, 63, 54, 193, 206, 140, 140, 152, 17, 19, 0, 58, 194, 55, 160, 113, 233, 103, 246, 8, 76, 202, 201, 3, 43, 241, 125, 14, 190, 253, 105, 170, 190, 78, 25, 186, 251, 29, 122, 33, 173, 160, 196, 16, 24, 86, 20, 136, 132, 217, 217, 116, 224, 150, 186, 18, 114, 12, 49, 239, 77, 93, 100, 41, 232, 103, 15, 117 };
            var result = provider.GetDecryptString(bytes);

            Assert.AreEqual("ersadmin", result);
        }

        [TestMethod]
        public void Test_Create_Clinic_Data()
        {
            var setup = new AdminContextSetup();
            var context = setup.SeedAdminContext();

            ClinicService service = new ClinicService(context, new MemoryCache(new MemoryCacheOptions()), setup.CertProvider);
            service.CreateClinicCache();

            var result = service.Get("0000");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Test 1", result.Value.Name);
            Assert.AreEqual("T0000", result.Value.Password);
        }

        [TestMethod]
        public void Test_Get_Clinic_Data_No_Password()
        {
            var setup = new AdminContextSetup();
            var context = setup.SeedAdminContext();

            ClinicService service = new ClinicService(context, new MemoryCache(new MemoryCacheOptions()), setup.CertProvider);
            service.CreateClinicCache();

            var result = service.Get("0001");

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("Test 2", result.Value.Name);
            Assert.IsNull(result.Value.Password);
        }
    }
}
