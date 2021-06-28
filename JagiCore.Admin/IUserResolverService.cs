using System.Collections.Generic;
using JagiCore.Admin.Data;

namespace JagiCore.Admin
{
    public interface IUserResolverService
    {
        ApplicationUser GetUser();

        /// <summary>
        /// 使用者預設的 Clinics，這個跟 GetClinics 最大差異是不經由 Group TABLE 的定義，直接由 Clinic/診所 中設定
        /// </summary>
        List<Clinic> GetDefaultClinics();

        /// <summary>
        /// 由 Group 中所對應的取出 Clinics
        /// </summary>
        List<Clinic> GetClinics();

        /// <summary>
        /// 由 Group 中所對應的取出 Clinics 會加入 Clinic 的設定（如果沒有時候）
        /// </summary>
        List<string> GetClinicCodes();

        /// <summary>
        /// 依據使用者的 GroupCode 作為層級設定，目前 GroupCode 如下：
        /// 010101 -> 院所代碼，如果 User Role = User 則為 Initail 最低的使用者，否則為機構主管
        /// 0101   -> 區經理
        /// 01     -> 處長
        /// </summary>
        /// <returns>1. Initial 2. Manager 3. Section Manager 4. Director</returns>
        string GetUserGroupLevel();

        /// <summary>
        /// 由 Group 中所對應的取出 Clinics 會加入 Clinic 的設定（如果沒有時候）
        /// </summary>
        Dictionary<string, string> GetClinicCodeNames();
        IList<string> GetRoles();

        /// <summary>
        /// 因系統紀錄為使用者登入帳號（Email），可以使用此方式由 Email 轉出 DisplayName
        /// </summary>
        /// <param name="username">UserName / Email</param>
        /// <returns></returns>
        string GetUserName(string username);
        string GetDisplayName(string email);
    }
}