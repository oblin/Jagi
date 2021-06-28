using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagiCore.Admin.Data;
using JagiCore.Helpers;
using Microsoft.EntityFrameworkCore;

namespace JagiCore.Admin.Tests.Data
{
    public class AdminContextSetup
    {
        public CertCrypto CertProvider { get; }

        public AdminContextSetup()
        {
            CertProvider = new CertCrypto("kiditCA");
        }

        public AdminContext SeedAdminContext()
        {
            var options = new DbContextOptionsBuilder<AdminContext>()
                              .UseInMemoryDatabase(Guid.NewGuid().ToString())
                              .Options;
            var context = new AdminContext(options);

            foreach (var clinic in GetDefaultClinics())
                context.Clinics.Add(clinic);

            context.SaveChanges();

            return context;
        }

        private IEnumerable<Clinic> GetDefaultClinics()
        {
            string password = "T0000";

            return new List<Clinic>
            {
                //new Clinic { Code = "0000", Name = "Test 1", EncryptPassword =  CertProvider.GetEncryptString(password) },
                new Clinic { Code = "0001", Name = "Test 2" }
            };
        }
    }
}
