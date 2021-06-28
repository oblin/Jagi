using System.Collections.Generic;
using System.Linq;
using JagiCore.Admin.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace JagiCore.Admin
{
    public class UserResolverService : IUserResolverService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AdminContext _context;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;

        public UserResolverService(UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContext,
            AdminContext context,
            IMemoryCache cache,
            IConfiguration configuration)
        {
            _httpContext = httpContext;
            _userManager = userManager;
            _context = context;
            _cache = cache;
            _configuration = configuration;
        }

        /// <summary>
        /// 使用者預設的 Clinics，這個跟 GetClinics 最大差異是不經由 Group TABLE 的定義，直接由 Clinic/診所 中設定
        /// </summary>
        public List<Clinic> GetDefaultClinics()
        {
            if (_httpContext.HttpContext.User == null
                || _httpContext.HttpContext.User.Identity == null
                || !_httpContext.HttpContext.User.Identity.IsAuthenticated)
                return null;
            var user = _userManager.GetUserAsync(_httpContext.HttpContext.User).Result;

            var clinics = new List<Clinic>();
            foreach(var item in _context.ClinicUsers.Where(c => c.UserId == user.Id))
            {
                clinics.Add(_context.Clinics.Find(item.ClinicId));
            }

            return clinics;
        }

        /// <summary>
        /// 取出 Group 中所對應的 Clinics
        /// </summary>
        /// <returns></returns>
        public List<Clinic> GetClinics()
        {
            var currentUser = GetUser();
            return currentUser?.Clinics;
        }

        public ApplicationUser GetUser()
        {
            if (_httpContext.HttpContext.User == null
                || _httpContext.HttpContext.User.Identity == null
                || !_httpContext.HttpContext.User.Identity.IsAuthenticated)
                return null;
            var user = _userManager.GetUserAsync(_httpContext.HttpContext.User).Result;
            user.Clinics = GetUserClinics(user.Id, user.GroupCode);
            return user;
        }

        /// <summary>
        /// 取代 UserManager，減少 Injection 的數量
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public string GetDisplayName(string email)
        {
            if (string.IsNullOrEmpty(email)) return string.Empty;

            var user = _userManager.FindByNameAsync(email).Result;
            return user?.DisplayName;
        }

        /// <summary>
        /// 依據使用者的 GroupCode 作為層級設定，目前 GroupCode 如下：
        ///              代碼長度
        /// 總公司 :     0                  >1    總經理：不簽，與簽核無關
        /// 長照部 :     01                 > 2,3 對應： 5 副總已審閱：目前最高的簽核層級無關
        /// 長照處 :     0101               > 4,5 對應： 5 副總已審閱：應該是處長層級，但目前沒有這樣的職位，因此同樣是副總
        /// 喬嵩蘭 :     010101             > 6,7 對應： 4 處主管已審閱
        /// 八里區 :     01010105           > 8,9 對應： 3 區經理已審閱
        /// 觀海 :       0101010501         > 1,  對應角色是 User, 代表： 1 初始填寫 
        /// 八里佳醫 :   0101010502         > 2.  對應角色非 User，代表：2 主管已審閱 
        /// </summary>
        /// <returns>1. Initial 2. Manager 3. Section Manager 4. Director</returns>
        public string GetUserGroupLevel()
        {
            var user = _userManager.GetUserAsync(_httpContext.HttpContext.User).Result;
            var roles = _userManager.GetRolesAsync(user).Result;
            if (user.GroupCode.Length >= 10)
            {
                if (_userManager.IsInRoleAsync(user, "User").Result)
                    return "1";
                else
                    return "2";

            }
            else if (user.GroupCode.Length >= 8 && user.GroupCode.Length < 10)
            {
                return "3";
            }
            else if (user.GroupCode.Length >= 6 && user.GroupCode.Length < 8)
            {
                return "4";
            }
            else if (user.GroupCode.Length >= 2 && user.GroupCode.Length < 6)
            {
                return "5";
            }

            return string.Empty;
        }

        public List<string> GetClinicCodes()
        {
            //List<string> clinics = new List<string>();
            //var user = GetUser();
            //if (user != null && user.Clinics != null)
            //    foreach (var clinic in user.Clinics)
            //        clinics.Add(clinic.Code);

            //return clinics;
            return GetClinicCodeNames().Keys.ToList();
        }

        public Dictionary<string, string> GetClinicCodeNames()
        {
            Dictionary<string, string> clinics = new Dictionary<string, string>();
            var user = GetUser();
            if (user != null && user.Clinics != null)
            {
                foreach (var clinic in user.Clinics)
                    clinics.Add(clinic.Code, clinic.Name);
                // 04-03 Add for Sales Report
                // 2019-07-18 TODO: This will cause Weekly Income Errors
                foreach(var item in GetDefaultClinics())
                {
                    if (!clinics.Keys.Contains(item.Code))
                        clinics.Add(item.Code, item.Name);
                }
            }

            return clinics;
        }

        public IList<string> GetRoles()
        {
            return _userManager.GetRolesAsync(GetUser()).Result;
        }


        private List<Clinic> GetUserClinics(string userId, string groupCode)
        {
            List<Clinic> clinics;
            string userClinicKey = GenerateUserKey(userId);
            if (_cache.TryGetValue(userClinicKey, out clinics))
                return clinics;

            clinics = new List<Clinic>();
            clinics = SetGroupClinics(groupCode, clinics);
            var cacheService = new CacheService(_context, _cache, _configuration);
            cacheService.AddUserClinic(userClinicKey, clinics);

            return clinics;
        }

        private string GenerateUserKey(string userId)
        {
            return _configuration["Constants:UserClinicKeyPrefix"] + userId;
        }

        private List<Clinic> SetGroupClinics(string groupCode, List<Clinic> clinics)
        {
            // TODO: 每讀一次就需要重新到資料庫讀取，這是錯誤的做法；改用 Cache 儲存 clinics 先到 Cache 讀取優先
            if (string.IsNullOrEmpty(groupCode))
                return clinics;

            var group = _context.Groups.Include(g => g.Groups).FirstOrDefault(g => g.Code == groupCode);
            if (group.Groups == null || !group.Groups.Any())
            {
                var clinic = _context.Clinics.FirstOrDefault(c => c.Code == group.ClinicCode);
                if (clinic != null)
                {
                    if (!clinics.Any(c => c.Id == clinic.Id))
                        clinics.Add(clinic);
                }
                return clinics;
            }
            else
            {
                foreach (var item in group.Groups)
                    SetGroupClinics(item.Code, clinics);

                return clinics;
            }
        }

        public string GetUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
                return username;

            var user = _userManager.FindByNameAsync(username)?.Result;
            if (user != null)
                return user.DisplayName;

            return username;
        }
    }
}
