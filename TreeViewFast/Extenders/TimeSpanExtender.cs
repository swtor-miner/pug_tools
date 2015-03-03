using System;


namespace TreeViewFast.Extenders
{
    public static class TimeSpanExtender
    {
        /// <summary>
        /// Present timespan in friendly form adequate for the size.
        /// </summary>
        public static string Display(this TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return string.Format("{0:N1} days", timeSpan.TotalDays);
            if (timeSpan.TotalHours >= 1)
                return string.Format("{0:N1} h", timeSpan.TotalHours);
            if (timeSpan.TotalMinutes >= 1)
                return string.Format("{0:N1} min", timeSpan.TotalMinutes);
            if (timeSpan.TotalSeconds >= 1)
                return string.Format("{0:N1} s", timeSpan.TotalSeconds);
            if (timeSpan.TotalMilliseconds >= 10)
                return string.Format("{0:N0} ms", timeSpan.TotalMilliseconds);
            if (timeSpan.TotalMilliseconds >= 1)
                return string.Format("{0:N1} ms", timeSpan.TotalMilliseconds);
            double totalMicroseconds = timeSpan.TotalMilliseconds * 1000;
            if (totalMicroseconds >= 10)
                return string.Format("{0:N0} μs", totalMicroseconds);
            return string.Format("{0:N1} μs", totalMicroseconds);
        }
    }
}