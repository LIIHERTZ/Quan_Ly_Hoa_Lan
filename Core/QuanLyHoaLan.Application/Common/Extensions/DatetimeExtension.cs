using QuanLyHoaLan.Application.Common.Models;

namespace QuanLyHoaLan.Application.Common.Extensions;

public static class DatetimeExtension
{
    public static long NowSeconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public static long TotalSeconds(this DateTime dateTime)
    {
        var utcDateTime = dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : dateTime.ToUniversalTime();
        var dateTimeOffset = new DateTimeOffset(utcDateTime);
        return dateTimeOffset.ToUnixTimeSeconds();
    }

    public static DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }

    public static DatetimeQueryDto? DatetimeQuery(this DateTime? dateTime)
    {
        if (dateTime == null || dateTime == default(DateTime))
            return null;

        var utcDateTime = dateTime.Value.Kind == DateTimeKind.Utc
            ? dateTime.Value
            : dateTime.Value.ToUniversalTime();

        var startOfDay = new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, 0, 0, 0, DateTimeKind.Utc);
        var endOfDay = startOfDay.AddDays(1).AddMilliseconds(-1);

        return new DatetimeQueryDto
        {
            StartDateAt = startOfDay,
            EndDateAt = endOfDay
        };
    }

    public static DateTime ToUtc(this DateTime dateTime)
    {
        return dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : DateTime.SpecifyKind(dateTime.ToUniversalTime(), DateTimeKind.Utc);
    }

    public static DateTime? ToUtc(this DateTime? dateTime)
    {
        return dateTime?.ToUtc();
    }

    public static DateTime StartOfDayUtc(this DateTime dateTime)
    {
        var utcDateTime = dateTime.ToUtc();
        return new DateTime(utcDateTime.Year, utcDateTime.Month, utcDateTime.Day, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime EndOfDayUtc(this DateTime dateTime)
    {
        return dateTime.StartOfDayUtc().AddDays(1).AddMilliseconds(-1);
    }

    public static DateTime StartOfMonthUtc(this DateTime dateTime)
    {
        var utcDateTime = dateTime.ToUtc();
        return new DateTime(utcDateTime.Year, utcDateTime.Month, 1, 0, 0, 0, DateTimeKind.Utc);
    }

    public static DateTime EndOfMonthUtc(this DateTime dateTime)
    {
        return dateTime.StartOfMonthUtc().AddMonths(1).AddMilliseconds(-1);
    }
}
