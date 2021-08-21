using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Security
{
    public static class Extensions
    {
        public static string AesEncrypt(this string clearText)
        {
            return new AesCryptography().Encrypt(clearText);
        }

        public static string AesDecrypt(this string cipherText)
        {
            return new AesCryptography().Decrypt(cipherText);
        }
    }
}
