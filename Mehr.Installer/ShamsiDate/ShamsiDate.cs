using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mehr.Installer
{
    public static class ShamsiDate
    {
        public static string GetCurrentYear()
        {
            PersianCalendar pc = new PersianCalendar();
            return pc.GetYear(DateTime.Now).ToString();
        }

        public static string ToShamsi(this DateTime dateTime)
        {
            PersianCalendar persian = new PersianCalendar();
            DateTime dt = DateTime.Now;
            string year = persian.GetYear(dt).ToString();
            string month = persian.GetMonth(dt).ToString("00");
            string day = persian.GetDayOfMonth(dt).ToString("00");
            return $"{year}/{month}/{day}";
        }
    }
}
