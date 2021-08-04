using Sepidar.Framework;
using Sepidar.Framework.Extensions;
using System;

namespace Sepidar.Console
{
    class ConsoleConfig : Config
    {
        public static int Width
        {
            get
            {
                if (HasSetting("Width"))
                {
                    string width = GetSetting("Width");
                    if (width.IsNumeric() && Convert.ToInt32(width) < 120)
                    {
                        return Convert.ToInt32(width);
                    }
                }
                return 100;
            }
        }

        public static int Height
        {
            get
            {
                if (HasSetting("Height"))
                {
                    string height = GetSetting("Height");
                    if (height.IsNumeric() && Convert.ToInt32(height) < 50)
                    {
                        return Convert.ToInt32(height);
                    }
                }
                return 30;
            }
        }

        public static int Records
        {
            get
            {
                if (HasSetting("Records"))
                {
                    string records = GetSetting("Records");
                    if (records.IsNumeric() && Convert.ToInt32(records) < 1000)
                    {
                        return Convert.ToInt32(records);
                    }
                }
                return 1000;
            }
        }

        public static bool Wrap
        {
            get
            {
                if (HasSetting("Wrap"))
                {
                    string wrap = GetSetting("Wrap");
                    if (wrap.ToLower() == "true")
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}