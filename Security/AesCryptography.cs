using Sepidar.Framework;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Sepidar.Security
{
    public class AesCryptography
    {
        private ICryptoTransform encryptor, decryptor;
        private UTF8Encoding encoder;

        public AesCryptography()
        {
            RijndaelManaged rm = new RijndaelManaged();
            encryptor = rm.CreateEncryptor(Config.AesKey, Config.AesVector);
            decryptor = rm.CreateDecryptor(Config.AesKey, Config.AesVector);
            encoder = new UTF8Encoding();
        }

        public string Encrypt(string clearText)
        {
            return Convert.ToBase64String(Encrypt(encoder.GetBytes(clearText)));
        }

        public string Decrypt(string cipherText)
        {
            return encoder.GetString(Decrypt(Convert.FromBase64String(cipherText)));
        }

        public byte[] Encrypt(byte[] buffer)
        {
            MemoryStream encryptStream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(encryptStream, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return encryptStream.ToArray();
        }

        public byte[] Decrypt(byte[] buffer)
        {
            MemoryStream decryptStream = new MemoryStream();
            using (CryptoStream cs = new CryptoStream(decryptStream, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(buffer, 0, buffer.Length);
            }
            return decryptStream.ToArray();
        }
    }
}
