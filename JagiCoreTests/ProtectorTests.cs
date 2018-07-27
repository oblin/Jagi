using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class ProtectorTests
    {
        [Fact]
        public void Can_Protect_Normal_Text()
        {
            string text = "Test String";
            var protector = ProtectorProvider.Default;
            string protectedText = protector.Protect(text);
            string unprotectedText = protector.Unprotect(protectedText);

            Assert.Equal(text, unprotectedText);
        }

        [Fact]
        public void Can_Protect_Normal_Chinese_Text()
        {
            string text = "中文字串";
            var protector = ProtectorProvider.Default;
            string protectedText = protector.Protect(text);
            string unprotectedText = protector.Unprotect(protectedText);

            Assert.Equal(text, unprotectedText);
        }

        [Fact]
        public void Default_Protector_With_Specified_Location()
        {
            string location = Directory.GetCurrentDirectory() + @"\DataProtectorSample";
            if (Directory.Exists(location))
                foreach (var file in (new DirectoryInfo(location)).GetFiles())
                    file.Delete();

            string text = "Test String";
            var protector = new ProtectorProvider(location);
            string protectedText = protector.Protect(text);
            string unprotectedText = protector.Unprotect(protectedText);
            
            Assert.Equal(text, unprotectedText);
            Assert.True(Directory.GetFiles(location).Count() > 0);
        }

        [Fact]
        public void Can_Protect_Within_Limited_Time()
        {
            string text = "中文字串";
            var protector = TimedProtectorProvider.Default;
            string protectedText = protector.Protect(text, TimeSpan.FromSeconds(3));
            string unprotectedText = protector.Unprotect(protectedText);

            Assert.Equal(text, unprotectedText);
        }

        [Fact]
        public void Exception_Protect_Out_Of_Limited_Time()
        {
            string text = "中文字串";
            var protector = TimedProtectorProvider.Default;
            string protectedText = protector.Protect(text, TimeSpan.FromSeconds(3));
            Thread.Sleep(3000);
            Assert.Throws<CryptographicException>(() => protector.Unprotect(protectedText));
        }
    }
}
