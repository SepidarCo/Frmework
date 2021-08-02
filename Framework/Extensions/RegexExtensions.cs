using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Framework.Extensions
{
    public static class RegexExtensions
    {
        public static void ReplaceInFiles(this Regex regex, string path, string newValue)
        {
            var content = File.ReadAllText(path);
            var newContent = regex.Replace(content, newValue);
            File.WriteAllText(path, newContent);
        }
    }
}
