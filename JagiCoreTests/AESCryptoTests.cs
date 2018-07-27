using JagiCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JagiCoreTests
{
    public class AESCryptoTests
    {
        [Fact]
        public void Test_Encrypt_Text()
        {
            string text = "Test";
            string key = "1234";
            string iv = "4321";

            var aes = new AESCrypto(key, iv);
            var encrypted = aes.Encrypt(text);

            Assert.NotEqual(text, encrypted);
            Assert.True(encrypted.Length > 8);

            var decrypted = aes.Decrypt(encrypted);

            Assert.Equal(text, decrypted);
        }

        [Fact]
        public void Test_Encrypt_Chinese_Text()
        {
            string text = "測試中文";
            string key = "1234";
            string iv = "4321";

            var aes = new AESCrypto(key, iv);
            var encrypted = aes.Encrypt(text);

            Assert.NotEqual(text, encrypted);
            Assert.True(encrypted.Length > 16);

            var decrypted = aes.Decrypt(encrypted);

            Assert.Equal(text, decrypted);
        }

        [Fact]
        public void Test_Encrypt_Chinese_Text_Length()
        {
            string text = "測試中文";
            string key = "1234";
            string iv = "4321";

            var aes = new AESCrypto(key, iv);
            var encrypted = aes.Encrypt(text);

            Assert.NotEqual(text, encrypted);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 24);
            Assert.True(encrypted.Length == 24);

            text = "測試中文字串";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 24);
            Assert.True(encrypted.Length == 24);

            key = "12345678";
            iv = "87654321";
            aes = new AESCrypto(key, iv);
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 24);
            Assert.True(encrypted.Length == 24);

            key = "1234567887654321";
            iv = "8765432112345678";
            aes = new AESCrypto(key, iv);
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 24);
            Assert.True(encrypted.Length == 24);

            text = "123456";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length == 24);

            text = "一二三四五六七八";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 48);
            Assert.True(encrypted.Length == 44);

            text = "一二三四五六七八九十";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 48);
            Assert.True(encrypted.Length == 44);

            text = "一二三四五六七八九十十二";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 48);
            Assert.True(encrypted.Length == 44);

            text = "一二三四五六七八九十十一十四";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 48);
            Assert.True(encrypted.Length == 44);

            text = "12345678901234";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length == 44);

            text = "一二三四五六七八九十十一十二十六";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 64);
            Assert.True(encrypted.Length == 64);

            text = "一二三四五六七八九十十一十二十六十八";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 64);
            Assert.True(encrypted.Length == 64);

            text = "一二三四五六七八九十十一十二十六十八二十";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 64);
            Assert.True(encrypted.Length == 64);

            text = "一二三四五六七八九十十一十二十六十八二十二二";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 64);
            Assert.True(encrypted.Length == 64);

            text = "1234567890123456789012";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length == 64);


            text = "一二三四五六七八九十十一十二十六十八二十二二二四";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 88);
            Assert.True(encrypted.Length == 88);

            text = "一二三四五六七八九十十一十二十六十八二十二二二四二六";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 88);
            Assert.True(encrypted.Length == 88);

            text = "一二三四五六七八九十十一十二十六十八二十二二二四二六二八";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 88);
            Assert.True(encrypted.Length == 88);

            text = "一二三四五六七八九十十一十二十六十八二十二二二四二六二八三十";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length >= 16);
            Assert.True(encrypted.Length <= 88);
            Assert.True(encrypted.Length == 88);

            text = "123456789012345678901234567890";
            encrypted = aes.Encrypt(text);
            Assert.True(encrypted.Length == 88);
        }
    }
}
