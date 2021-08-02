using Sepidar.Framework.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Sepidar.Framework
{
    public static class PersianDateTime
    {
        static DateTimeFormatInfo dateTimeFormatInfo = DateTimeFormatInfo.CurrentInfo;
        static Calendar calendar = new GregorianCalendar();
        static PersianCalendar persianCalendar = new PersianCalendar();

        public static string ToPersianDate(this DateTime latinDate, string separator = "/")
        {
            PersianCalendar pc = new PersianCalendar();
            string year = pc.GetYear(latinDate).ToString();
            string month = pc.GetMonth(latinDate).ToString();
            string day = pc.GetDayOfMonth(latinDate).ToString();
            month = month.Length == 2 ? month : month.PadLeft(2, '0');
            day = day.Length == 2 ? day : day.PadLeft(2, '0');
            return "{0}{1}{2}{1}{3}".Fill(year, separator, month, day);
        }

        public static string ToPersianDateTime(this DateTime latinDate, string separator = "/")
        {
            return "{0} - {1}".Fill(latinDate.ToPersianDate(separator), latinDate.ToString("H:mm"));
        }

        public static DateTime Parse(int year, int month, int day, int hour, int minute, int second)
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.ToDateTime(year, month, day, hour, minute, second, 0);
        }

        public static bool IsPersianDate(this string value)
        {
            return Regex.Match(value, RegularExpressions.PersianDateTime).Success;
        }

        public static DateTime Parse(string persianDate)
        {
            if (!IsPersianDate(persianDate))
            {
                throw new FrameworkException("'{0}' is not a valid Persian date".Fill(persianDate));
            }
            else
            {
                var match = Regex.Match(persianDate, RegularExpressions.PersianDateTime);
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

        public static int GetWeekOfMonth(DateTime date)
        {
            DateTime firstDay = new DateTime(date.Year, date.Month, 1);
            var week = calendar.GetWeekOfYear(date, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek) - calendar.GetWeekOfYear(firstDay, dateTimeFormatInfo.CalendarWeekRule, dateTimeFormatInfo.FirstDayOfWeek) + 1;
            return week;
        }

        public static string GetPersianDayOfWeekName(this DateTime latinDate)
        {
            switch (latinDate.DayOfWeek)
            {
                case DayOfWeek.Saturday: return "شنبه";
                case DayOfWeek.Sunday: return "يکشنبه";
                case DayOfWeek.Monday: return "دوشنبه";
                case DayOfWeek.Tuesday: return "سه‏ شنبه";
                case DayOfWeek.Wednesday: return "چهارشنبه";
                case DayOfWeek.Thursday: return "پنجشنبه";
                case DayOfWeek.Friday: return "جمعه";
                default: throw new FrameworkException("Invalid day of week {0}".Fill(latinDate.DayOfWeek));
            }
        }

        public static string GetPersianMonthName(this DateTime date)
        {
            string persianDate = date.ToPersianDate();
            string[] dateParts = persianDate.Split('/');
            int month = Convert.ToInt32(dateParts[1]);

            switch (month)
            {
                case 1: return "فروردین";
                case 2: return "اردیبهشت";
                case 3: return "خرداد";
                case 4: return "تیر‏";
                case 5: return "مرداد";
                case 6: return "شهریور";
                case 7: return "مهر";
                case 8: return "آبان";
                case 9: return "آذر";
                case 10: return "دی";
                case 11: return "بهمن";
                case 12: return "اسفند";
                default: throw new FrameworkException("Invalid persian month {0} for {1}".Fill(month, date.ToString()));
            }
        }

        public static int GetPersianDayOfMonth(this DateTime date)
        {
            return persianCalendar.GetDayOfMonth(date);
        }

        public static int GetPersianDayOfYear(this DateTime date)
        {
            return persianCalendar.GetDayOfYear(date);
        }

        public static int GetPersianMonth(this DateTime date)
        {
            return persianCalendar.GetMonth(date);
        }

        public static int GetPersianWeekOfYear(this DateTime date)
        {
            return persianCalendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Saturday);
        }

        public static int GetPersianYear(this DateTime date)
        {
            return persianCalendar.GetYear(date);
        }

        public static int GetPersianWeekOfMonth(this DateTime date)
        {
            //var dayCounter = Parse(date.GetPersianYear(), date.GetPersianMonth(), 1, 0, 0, 0);
            //int week = 1;
            //while (dayCounter <= date)
            //{
            //    if (dayCounter.Day == (int)DayOfWeek.Friday)
            //    {
            //        ++week;
            //    }
            //    dayCounter = dayCounter.AddDays(1);
            //}
            //return week;
            var firstDate = Parse(date.GetPersianYear(), date.GetPersianMonth(), 1, 0, 0, 0);
            var week = date.GetPersianWeekOfYear() - firstDate.GetPersianWeekOfYear() + 1;
            return week;
        }

        public static string GetPersianDatePart(this string value)
        {
            return value.Substring(0, 10);
        }

        public static int GetPersianDaysInMonth(this DateTime dateTime)
        {
            return persianCalendar.GetDaysInMonth(GetPersianYear(dateTime), GetPersianMonth(dateTime));
        }

        public static int GetPersianDaysInMonth(this int persianYear, int persianMonth)
        {
            return persianCalendar.GetDaysInMonth(persianYear, persianMonth);
        }
    }
}
