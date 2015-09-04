using System;
using System.Collections.Generic;
using System.Text;

namespace VLF.CLS
{
    public class DateSpan
    {
        private DateSpan() { }

        public enum SpanType
        {
            Hourly,
            Daily,
            Weekly,
            BiWeekly,
            Monthly,
            BiMonthly,
            Quarterly,
            SemiAnnually,
            Annually,
            BiAnnually,
        }

        public static int Get(SpanType spanType, DateTime timestamp)
        {
            DateTime future = timestamp;
            switch (spanType)
            {
                case SpanType.Hourly: future = timestamp.AddHours(1); break;
                case SpanType.Daily: future = timestamp.AddDays(1); break;
                case SpanType.Weekly: future = timestamp.AddDays(7); break;
                case SpanType.BiWeekly: future = timestamp.AddDays(14); break;
                case SpanType.Monthly: future = timestamp.AddMonths(1); break;
                case SpanType.BiMonthly: future = timestamp.AddMonths(2); break;
                case SpanType.Quarterly: future = timestamp.AddMonths(3); break;
                case SpanType.SemiAnnually: future = timestamp.AddMonths(6); break;
                case SpanType.Annually: future = timestamp.AddYears(1); break;
                case SpanType.BiAnnually: future = timestamp.AddYears(2); break;
            }
            return (int)Math.Ceiling((future - timestamp).TotalHours);
        }

        public static int Get(SpanType spanType, DateTime timestamp, out DateTime future)
        {
            future = timestamp;
            switch (spanType)
            {
                case SpanType.Hourly: future = timestamp.AddHours(1); break;
                case SpanType.Daily: future = timestamp.AddDays(1); break;
                case SpanType.Weekly: future = timestamp.AddDays(7); break;
                case SpanType.BiWeekly: future = timestamp.AddDays(14); break;
                case SpanType.Monthly: future = timestamp.AddMonths(1); break;
                case SpanType.BiMonthly: future = timestamp.AddMonths(2); break;
                case SpanType.Quarterly: future = timestamp.AddMonths(3); break;
                case SpanType.SemiAnnually: future = timestamp.AddMonths(6); break;
                case SpanType.Annually: future = timestamp.AddYears(1); break;
                case SpanType.BiAnnually: future = timestamp.AddYears(2); break;
            }
            return (int)Math.Ceiling((future - timestamp).TotalHours);
        }

    }
}
