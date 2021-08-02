using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class ColorExtensions
    {
        public static Color Invert(this Color color)
        {
            return Color.FromArgb(255 - color.R, 255 - color.G, 255 - color.B);
        }
    }
}
