using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System.Text;

namespace JagiCore.Helpers
{
    /// <summary>
    /// 使用 DataProtector 進行資料的加密，在 Linux 中必須要設定 LOCALAPPDATA 指向儲存的路徑
    /// 這裡，若要使用 none persist，請自行建立
    /// </summary>
    public class ProtectorProvider
    {
        private IDataProtector _defaultProtector { get; }
        private static ProtectorProvider _defaultInstance;
        private const string DEFAULT_PURPOSE = "Jagi.Persist.Default";
        private const string NON_PERSIST_PURPOSE = "Jagi.NotPersist.Default";

        private ProtectorProvider()
        {
            var destDir = Path.Combine(
                 Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                 "AppSecrets");

            this._defaultProtector = GenerateProtectProvider(destDir, DEFAULT_PURPOSE, true);
        }

        /// <summary>
        /// 提供選項可以自行設定是否 persist & purpose
        /// </summary>
        public ProtectorProvider(string destDirectory, bool persist = true, string purpose = null)
        {
            purpose = purpose ?? (persist ? DEFAULT_PURPOSE : NON_PERSIST_PURPOSE);

            this._defaultProtector = GenerateProtectProvider(destDirectory, purpose, persist);
        }

        /// <summary>
        /// 預設為 persist key management
        /// </summary>
        public static ProtectorProvider Default = _defaultInstance ?? (_defaultInstance = new ProtectorProvider());

        public string Unprotect(string protectedText)
        {
            return _defaultProtector.Unprotect(protectedText);
        }

        public string Protect(string text)
        {
            //byte[] input = Encoding.UTF8.GetBytes(text);
            //return Convert.ToBase64String(_defaultProtector.Protect(input));
            return _defaultProtector.Protect(text);
        }

        private IDataProtector GenerateProtectProvider(string destDir, string purpose, bool persist)
        {
            var serviceColletion = new ServiceCollection();
            var builder = serviceColletion.AddDataProtection();
            if (persist)
                // 設定儲存的位置
                builder = builder.PersistKeysToFileSystem(new DirectoryInfo(destDir));

            var services = serviceColletion.BuildServiceProvider();

            // 取得 protector
            return services.GetDataProtector(purpose);
        }
    }
}
