using System;

namespace Tranglo1.CustomerIdentity.Domain.Common.Extensions
{
    public static partial class Extensions
    {
        public static DateTime UTCToMalaysiaTime(this DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return DateTime.MinValue;
            }

            return dateTime.Value.UTCToMalaysiaTime();
        }

        public static DateTime UTCToMalaysiaTime(this DateTime dateTime)
        {
            var malaysiaTime = new DateTimeOffset(dateTime, TimeSpan.Zero)
                .ToOffset(TimeSpan.FromHours(8))
                .DateTime;

            return malaysiaTime;
        }
    }
}
