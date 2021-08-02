using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Sepidar.Framework
{
    public static class ColorHelper
    {
        public static Color RedToGreenPercentage(double percent)
        {
            // HSL => H from 0 to 360 => 0 & 360 on the wheel = red, 120 = green, 240 = blue;
            percent = (percent * 120) / 100;
            var hue = percent / 360;
            return FromHsl(hue, 1, 0.5);
        }

        public static Color FromHsl(double hue, double saturation, double luminance)
        {
            if (hue < 0 || hue > 1)
            {
                throw new FrameworkException("Hue should be between 0 and 1");
            }
            if (saturation < 0 || saturation > 1)
            {
                throw new FrameworkException("Saturation should be between 0 and 1");
            }
            if (luminance < 0 || luminance > 1)
            {
                throw new FrameworkException("Luminance should be between 0 and 1");
            }
            double red = 0, green = 0, blue = 0;
            if (luminance != 0)
            {
                if (saturation == 0)
                    red = green = blue = luminance;
                else
                {
                    double temp2;
                    if (luminance < 0.5)
                        temp2 = luminance * (1.0 + saturation);
                    else
                        temp2 = luminance + saturation - (luminance * saturation);

                    double temp1 = 2.0 * luminance - temp2;

                    red = GetColorComponent(temp1, temp2, hue + 1.0 / 3.0);
                    green = GetColorComponent(temp1, temp2, hue);
                    blue = GetColorComponent(temp1, temp2, hue - 1.0 / 3.0);
                }
            }
            return Color.FromArgb((int)(255 * red), (int)(255 * green), (int)(255 * blue));

        }

        private static double GetColorComponent(double temp1, double temp2, double temp3)
        {
            if (temp3 < 0.0)
                temp3 += 1.0;
            else if (temp3 > 1.0)
                temp3 -= 1.0;

            if (temp3 < 1.0 / 6.0)
                return temp1 + (temp2 - temp1) * 6.0 * temp3;
            else if (temp3 < 0.5)
                return temp2;
            else if (temp3 < 2.0 / 3.0)
                return temp1 + ((temp2 - temp1) * ((2.0 / 3.0) - temp3) * 6.0);
            else
                return temp1;
        }

        public static string ToHex(this Color color)
        {
            return "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }

        public static string ToRgb(this Color color)
        {
            return "RGB(" + color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString() + ")";
        }
    }
}
