using System;
using System.IO;
using System.Linq;
using Sepidar.Framework.Extensions;

namespace Sepidar.BlobManagement
{
    public class BlobHelper
    {
        public static string TurnIntoWebUrl(string token, string extension, string host, bool useLoadBalancing = false)
        {
            token = NormalizeToken(token);
            extension = NormalizeExtension(extension);
            var alphaNumerics = token.ToCharArray().Select(i => i.ToString()).ToArray();
            int staticsHostNumber = Int32.Parse(alphaNumerics.First(), System.Globalization.NumberStyles.HexNumber);
            var filePath = String.Join("/", alphaNumerics);
            Uri hostName = null;
            if (useLoadBalancing)
            {
                hostName = new Uri(host.Replace("statics", "statics" + staticsHostNumber));
            }
            else
            {
                hostName = new Uri(host);
            }
            var result = new Uri(hostName, filePath + "/file." + extension);
            return result.AbsoluteUri;
        }

        public static string TurnIntoDiskPath(string token, string extension, string root = null)
        {
            token = NormalizeToken(token);
            extension = NormalizeExtension(extension);
            var filePath = string.Join("\\", token.ToCharArray().Select(i => i.ToString()).ToArray());
            if (root.IsSomething())
            {
                filePath = Path.Combine(root, filePath);
            }
            filePath = filePath + "\\file." + extension;
            return filePath;
        }

        private static string NormalizeExtension(string extension)
        {
            extension = extension.Replace(".", "");
            return extension;
        }

        private static string NormalizeToken(string token)
        {
            token = token.Replace("-", "");
            return token;
        }
    }
}
