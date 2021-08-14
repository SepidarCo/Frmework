using Sepidar.Framework;
using System;

namespace Sepidar.Validation
{
    public static class PersianDateValidator
    {
        public static bool IsPersianDate(string date)
        {
            // Since there is no TryParse in PersianCalendar (like DateTime.TryParse), try/catch is used here.
            try
            {
                PersianDateTime.Parse(date);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
