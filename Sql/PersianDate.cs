using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Sql
{
    public class PersianDate
    {
        public const string Pattern = @"^(13\d{2})[-/](\d{1,2})[-/](\d{1,2})( (\d{1,2}):(\d{1,2}):(\d{1,2}))?$";

       // [SqlFunction]
        public static SqlString ToPersianDate(SqlDateTime latinDate)
        {
            if (latinDate.IsNull)
            {
                return null;
            }
            PersianCalendar pc = new PersianCalendar();
            string year = pc.GetYear(latinDate.Value).ToString();
            string month = pc.GetMonth(latinDate.Value).ToString();
            string day = pc.GetDayOfMonth(latinDate.Value).ToString();
            month = month.Length == 2 ? month : month.PadLeft(2, '0');
            day = day.Length == 2 ? day : day.PadLeft(2, '0');
            return string.Format("{0}{1}{2}{1}{3}", year, "/", month, day);
        }

        public static SqlString ToPersianDateTime(SqlDateTime latinDate)
        {
            if (latinDate.IsNull)
            {
                return null;
            }
            return string.Format("{0} - {1}", ToPersianDate(latinDate), latinDate.Value.ToString("HH:mm"));
        }

        public static DateTime Parse(int year, int month, int day, int hour, int minute, int second)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.ToDateTime(year, month, day, hour, minute, second, 0);
        }

        public static DateTime Parse(string persianDate)
        {
            var match = Regex.Match(persianDate, Pattern);
            if (match.Groups.Count == 7)
            {
                return Parse(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value), Convert.ToInt32(match.Groups[5].Value), Convert.ToInt32(match.Groups[6].Value), Convert.ToInt32(match.Groups[7].Value));
            }
            else
            {
                return Parse(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value), Convert.ToInt32(match.Groups[3].Value), 0, 0, 0);
            }
        }
    }
}
