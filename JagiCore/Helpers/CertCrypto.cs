using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace JagiCore.Helpers
{
    public class CertCrypto
    {
        private readonly RSA _provider;

        public CertCrypto(string subjectName)
        {
            X509Certificate2 cert = GetStoreKiditCert(subjectName);
            _provider = cert.GetRSAPrivateKey();
        }

        public string GetDecryptString(byte[] encrypt)
        {
            // Add for update
            if (encrypt == null)
                return string.Empty;

            //byte[] bteEncrypt = null;
            string strResault = null;
            byte[] bteDecrypt = null;

            bteDecrypt = _provider.Decrypt(encrypt, RSAEncryptionPadding.Pkcs1);
            strResault = Encoding.UTF8.GetString(bteDecrypt);
            return strResault;
        }

        public byte[] GetEncryptString(string crypt)
        {
            if (string.IsNullOrEmpty(crypt))
                return null;

            byte[] bteCrypt = null;
            byte[] bteResult = null;

            bteCrypt = Encoding.UTF8.GetBytes(crypt);
            bteResult = _provider.Encrypt(bteCrypt, RSAEncryptionPadding.Pkcs1);
            //return Encoding.UTF8.GetString(bteResult);
            return bteResult;
        }

        public X509Certificate2 GetStoreKiditCert(string certSubjectName)
        {
            X509Certificate2 cert = null;

            X509Store store = new X509Store("MY", StoreLocation.LocalMachine);
            store.Open(OpenFlags.OpenExistingOnly | OpenFlags.ReadOnly);

            X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, certSubjectName, false);

            if (certs != null && certs.Count > 0)
            {
                cert = certs[0];
            }

            return cert;
        }
    }
}
