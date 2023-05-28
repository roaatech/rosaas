namespace Roaa.Rosas.Common.Extensions
{
    public static class DateTimeExtentions
    {
        public static double TimeStamp(this DateTime date)
        {
            DateTime _date = new DateTime(1970, 1, 1);
            return date.TimeStamp(_date);
        }

        public static double TimeStamp(this DateTime date, DateTime fromDate)
        {
            DateTime _date = new DateTime(1970, 1, 1);
            TimeSpan ts = new TimeSpan(date.Ticks - _date.Ticks);
            return ts.TotalSeconds;
        }


        public static DateTime StartDateTime(this DateTime dateTime, int startTimeInHour, int timeZone, int timePeriodInHour = 24)
        {
            var date = dateTime.Date.AddHours(-timeZone).AddHours(startTimeInHour);

            if (DateTime.UtcNow <= date) date = date.AddHours(-timePeriodInHour);

            return date;
        }

        public static DateTime EndDateTime(this DateTime dateTime, int startTimeInHour, int timeZone, int timePeriodInHour = 24)
        {
            var date = dateTime.Date.AddHours(-timeZone).AddHours(startTimeInHour);

            if (DateTime.UtcNow > date) date = date.AddHours(timePeriodInHour);

            return date;
        }


        public static bool InclusiveBetween(this DateTime dateTime, DateTime startDate, DateTime endDate)
        {
            return dateTime >= startDate && dateTime <= endDate;
        }
    }
}

