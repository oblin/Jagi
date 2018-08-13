using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JagiCore.Admin.Data;
using JagiCore.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace JagiCore.Admin
{
    /// <summary>
    /// 主要是要將 Clinic 變成 Cache Service，啟動就將所有診所資料載入
    /// 降低資料庫存取
    /// </summary>
    public class ClinicService
    {
        private readonly AdminContext _context;
        private readonly IMemoryCache _cache;
        private readonly CertCrypto _cert;

        public ClinicService(AdminContext context, IMemoryCache cache, CertCrypto cert)
        {
            _context = context;
            _cache = cache;
            _cert = cert;
        }

        public void CreateClinicCache()
        {
            var clinics = _context.Clinics.ToList();
            foreach (var clinic in clinics)
            {
                if (clinic.EncryptDatabasePassword != null && clinic.EncryptDatabasePassword.Any())
                    if (_cert == null)
                        clinic.DatabasePassword = Convert.ToBase64String(clinic.EncryptDatabasePassword);
                    else
                        clinic.DatabasePassword = _cert.GetDecryptString(clinic.EncryptDatabasePassword);

                Add(clinic);
            }
        }

        public void Add(Clinic clinic)
        {
            string key = GetKey(clinic.Code);
            _cache.Set(key, clinic, new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
        }

        public void Update(Clinic clinic)
        {
            Remove(clinic);
            Add(clinic);
        }

        public Result<Clinic> Get(object code)
        {
            string key = GetKey(code);
            if (_cache.TryGetValue(key, out Clinic clinic))
                return Result.Ok(clinic);
            else
                return Result.Fail<Clinic>($"{code} 找不到機構資料");

        }

        public void Remove(Clinic clinic)
        {
            string key = GetKey(clinic.Code);
            _cache.Remove(key);
        }

        private string GetKey(object code)
        {
            return $"Clinic_{code.ToString().ToUpper()}";
        }
    }
}
