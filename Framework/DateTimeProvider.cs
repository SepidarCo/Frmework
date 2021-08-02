using System;
using System.Collections.Generic;
using System.Text;

namespace Sepidar.Framework
{
    public static class DateTimeProvider
    {
        public static Func<DateTime> Now = () => DateTime.Now;

        public static void SetDateTime(DateTime dateTimeNow)
        {
            Now = () => dateTimeNow;
        }

        public static void ResetDateTime()
        {
            Now = () => DateTime.Now;
        }
    }
}
