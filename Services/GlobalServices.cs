using NodaTime;
using System;
using System.Globalization;

namespace WebAPI_tutorial.Services
{
    public static class GlobalServices
    {
        public static DateTime GetDatetimeUruguay()
        {
            var nowInUruguay = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["America/Montevideo"]);
            return nowInUruguay.ToDateTimeUnspecified();
        }

        public static string GetDatetimeUruguayString()
        {
            CultureInfo culture = new CultureInfo("es-UY");
            var nowInUruguay = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["America/Montevideo"]);
            return nowInUruguay.ToDateTimeUnspecified().ToString("G", culture);
        }

    }
}
