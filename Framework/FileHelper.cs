using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Sepidar.Framework
{
    public class FileHelper
    {
        public static void WaitUntilIsAccessible(string path)
        {
            var isAccessible = false;
            while (!isAccessible)
            {
                try
                {
                    using (Stream stream = new FileStream(path, FileMode.Open))
                    {
                        isAccessible = true;
                    }
                }
                catch (Exception)
                {
                    // silenced by design => retry mechanism
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
