using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Sepidar.Security
{
    public class RsaCryptography
    {
        public static KeyPair CreatePrivatePublicKey()
        {
            var cspParams = new CspParameters(1);
            cspParams.KeyContainerName = "KeyContainer";
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.ProviderName = "Microsoft Strong Cryptographic Provider";
            var rsa = new RSACryptoServiceProvider(cspParams);
            var publicPrivateKeyXML = rsa.ToXmlString(true);
            var publicOnlyKeyXML = rsa.ToXmlString(false);
            return new KeyPair
            {
                PrivateKey = publicPrivateKeyXML,
                PublicKey = publicOnlyKeyXML
            };
        }

        public string Encrypt(string publicKeyXML, string clearText)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKeyXML);
            var cipherText = rsa.Encrypt(Encoding.ASCII.GetBytes(clearText), true).GetString();
            return cipherText;
        }

        public string Decrypt(string publicPrivateKeyXML, string cipherText)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicPrivateKeyXML);
            return Encoding.ASCII.GetString(rsa.Decrypt(cipherText.ToBytes(), true));
        }
    }
}
