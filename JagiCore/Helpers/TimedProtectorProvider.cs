using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JagiCore.Helpers
{
    /// <summary>
    /// 使用 DataProtector 進行資料的加密，在 Linux 中必須要設定 LOCALAPPDATA 指向儲存的路徑
    /// 可以指定 key 有效期限，可以用在加密 cookie 上
    /// </summary>
    public class TimedProtectorProvider
    {
        private ITimeLimitedDataProtector _defaultProtector { get; }
        private static TimedProtectorProvider _defaultInstance;
        private const string DEFAULT_PURPOSE = "Jagi.Timed.Default";
        private const string NON_PERSIST_PURPOSE = "Jagi.NotPersist.Timed.Default";

        private TimedProtectorProvider()
        {
            var destDir = Path.Combine(
                 Environment.GetEnvironmentVariable("LOCALAPPDATA"),
                 "AppSecrets");

            this._defaultProtector = GenerateProtectProvider(destDir, DEFAULT_PURPOSE, true);
        }

        /// <summary>
        /// 提供選項可以自行設定是否 persist & purpose
        /// </summary>
        public TimedProtectorProvider(string destDirectory, bool persist = true, string purpose = null)
        {
            purpose = purpose ?? (persist ? DEFAULT_PURPOSE : NON_PERSIST_PURPOSE);

            this._defaultProtector = GenerateProtectProvider(destDirectory, purpose, persist);
        }

        /// <summary>
        /// 預設為 persist key management
        /// </summary>
        public static TimedProtectorProvider Default = _defaultInstance ?? (_defaultInstance = new TimedProtectorProvider());

        public string Unprotect(string protectedText)
        {
            return _defaultProtector.Unprotect(protectedText);
        }

        public string Protect(string text, TimeSpan? lifeTime = null)
        {
            if (lifeTime == null)
                return _defaultProtector.Protect(text);
            else 
                return _defaultProtector.Protect(text, (TimeSpan)lifeTime);
        }

        private ITimeLimitedDataProtector GenerateProtectProvider(string destDir, string purpose, bool persist)
        {
            var serviceColletion = new ServiceCollection();
            var builder = serviceColletion.AddDataProtection();
            if (persist)
                // 設定儲存的位置
                builder = builder.PersistKeysToFileSystem(new DirectoryInfo(destDir));

            var services = serviceColletion.BuildServiceProvider();

            // 取得 protector
            return services.GetDataProtector(purpose).ToTimeLimitedDataProtector();
        }
    }
}
