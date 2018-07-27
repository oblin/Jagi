using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace JagiCore.Helpers
{
    /// <summary>
    /// 必須要指定 CryptoSetting，或者給予加解密的值，密碼可為任意長度，建議至少長度為16以上的字串
    /// 此算法比 DES 安全，並且速度要快上 10 倍，推薦使用
    /// 另外 key & iv 使用 128 bit or 256 bit 實際上速度差不多
    /// </summary>
    public class AESCrypto
    {
        private readonly string _key;
        private readonly string _iv;
        public AESCrypto(string key, string iv)
        {
            _key = key;
            _iv = iv;
        }

        /// <summary>
        /// 主要的加密函數，直接輸入 Encrypt 即可
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Encrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var aes = Aes.Create();

            var md5 = MD5.Create();

            byte[] plainTextData = Encoding.Unicode.GetBytes(text);

            byte[] keyData = md5.ComputeHash(Encoding.Unicode.GetBytes(_key));

            byte[] IVData = md5.ComputeHash(Encoding.Unicode.GetBytes(_iv));

            ICryptoTransform transform = aes.CreateEncryptor(keyData, IVData);

            byte[] output = transform.TransformFinalBlock(plainTextData, 0, plainTextData.Length);

            return Convert.ToBase64String(output);

        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="text">加密過的文字</param>
        /// <returns>傳回解密文字</returns>
        public string Decrypt(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            byte[] cipherTextData = Convert.FromBase64String(text);

            var aes = Aes.Create();

            var md5 = MD5.Create();

            byte[] keyData = md5.ComputeHash(Encoding.Unicode.GetBytes(_key));

            byte[] IVData = md5.ComputeHash(Encoding.Unicode.GetBytes(_iv));

            ICryptoTransform transform = aes.CreateDecryptor(keyData, IVData);

            byte[] output = transform.TransformFinalBlock(cipherTextData, 0, cipherTextData.Length);

            return Encoding.Unicode.GetString(output);

        }
    }
}
