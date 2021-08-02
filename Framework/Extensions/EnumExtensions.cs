using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sepidar.Framework.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            var descriptionAttribute = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (descriptionAttribute.Count() > 0)
            {
                return ((DescriptionAttribute)descriptionAttribute[0]).Description;
            }
            return string.Empty;
            //throw new FrameworkException("Either enum {0} doesn't have the [Description()] attribute, or another problem happened.".Fill(value.ToString()));

        }

        public static int GetValue(this Enum value)
        {
            return (int)value.GetType().GetField(value.ToString()).GetRawConstantValue();
        }


        /// <summary>
        /// علی تو این قسمت مقدار خروجی رو تغییر دادی
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToLdsw(this Enum value)
        {
            //return value.ToString().ToLdsw();
            return value.ToLdsw();
        }
    }
}
