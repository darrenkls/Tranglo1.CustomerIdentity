using CSharpFunctionalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeZoneNames;

namespace Tranglo1.CustomerIdentity.Domain.Common
{
    public static class TimezoneConversion
    {
        public static DateTime ConvertFromUTC(string regionCountry, string languageCode, DateTime dateTime)
        {
            
            var timeZoneValues = TZNames.GetNamesForTimeZone(regionCountry, languageCode);
    
            var timezoneName = TimeZoneInfo.FindSystemTimeZoneById(timeZoneValues.Standard);
            var result = TimeZoneInfo.ConvertTimeFromUtc(dateTime, timezoneName);

            return result;
        }

        public static Result<DateTime> ConvertFromUTCWithTimezoneName(string timezoneName, string languageCode, DateTime dateTime, bool omitException = false)
        {
            if (String.IsNullOrEmpty(timezoneName))
                return dateTime;

            var timeZoneValues = TZNames.GetDisplayNames(languageCode);
            var timezoneId = timeZoneValues.FirstOrDefault(x => x.Value == timezoneName)
                .Key;

            if (timezoneId == null)
            {
                if (omitException)
                    return dateTime;

                return Result.Failure<DateTime>("Invalid timezone name.");
            }

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            var result = TimeZoneInfo.ConvertTimeFromUtc(dateTime, tzi);

            return result;
        }
    }
}
