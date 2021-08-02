using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class ByteExtensions
    {
        public static string GetString(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
