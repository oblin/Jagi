using JagiCore.Admin.Data;
using JagiCore.Helpers;
using JagiCore.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Admin
{
    public class CacheService
    {
        private readonly AdminContext _context;
        private readonly CodeService _codeService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public CacheService(AdminContext context,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _context = context;
            _codeService = new CodeService(cache);
            _configuration = configuration;
            _cache = cache;
        }

        /// <summary>
        /// 將 CodeFile & CodeCache 從資料庫讀出後加入 CodeService 中
        /// </summary>
        public void CreateCodeCache()
        {
            var codes = _context.CodeFiles.Include(c => c.CodeDetails).ToList();
            foreach (var code in codes)
            {
                _codeService.Add(code);
            }
        }

        /// <summary>
        /// 清除 CodeService 並重新將 CodeFile & CodeCache 從資料庫讀出後加入
        /// </summary>
        public void RebuildCodeCache()
        {
            var codes = _context.CodeFiles.Include(c => c.CodeDetails).ToList();
            foreach (var code in codes)
            {
                _codeService.Update(code);
            }
        }

        public void ClearUserClinics()
        {
            var userClinicKeys = _configuration["Constants:UserClinicKeys"];
            _cache.TryGetValue(userClinicKeys, out List<string> userClinicsList);
            if (userClinicsList != null)
            {
                foreach (var key in userClinicsList)
                    _cache.Remove(key);
            }
            _cache.Remove(userClinicKeys);
        }

        /// <summary>
        /// 建立登入使用者與對應的機構代表列表
        /// </summary>
        /// <param name="userClinicKey">登入使用者ID</param>
        /// <param name="clinics">對應的機構列表</param>
        public void AddUserClinic(string userClinicKey, List<Clinic> clinics)
        {
            var userClinicKeys = _configuration["Constants:UserClinicKeys"];
            _cache.TryGetValue(userClinicKeys, out List<string> userClinicsList);

            if (userClinicsList == null)
                userClinicsList = new List<string>();

            userClinicsList.Add(userClinicKey);

            // 使用 UserClinicKeys 紀錄目前 cache 有哪些 user id 有對應的 clinics，因為以後要刪除
            _cache.Set(userClinicKeys, userClinicsList, new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
            // 建立 user id 與對應的 clinics
            _cache.Set(userClinicKey, clinics, new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove));
        }
    }
}
