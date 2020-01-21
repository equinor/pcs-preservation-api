﻿using System;

namespace Equinor.Procosys.Preservation.Domain
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, int weeks)
            => dateTime.AddDays(7 * weeks);

        public static int GetWeeksUntilDate(this DateTime fromDateTime, DateTime toDateTime)
        {
            if (fromDateTime.Kind != toDateTime.Kind)
            {
                throw new ArgumentException($"{nameof(fromDateTime)} and {nameof(toDateTime)} has different kinds");
            }
            
            var ts = toDateTime.Subtract(fromDateTime);
            return ts.Days/7;
        }
    }
}
