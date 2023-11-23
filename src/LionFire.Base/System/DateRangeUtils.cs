namespace LionFire.Base;

public class DateRangeUtils
{
    public static (DateTime start, DateTime end) GetMonths(DateTime date, int months = 1)
    {
        var start = new DateTime(date.Year, date.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var endExclusive = start ;
        for (int i = 0; i < months; i++)
        {
            endExclusive = endExclusive + TimeSpan.FromDays(31);
            endExclusive = new DateTime(endExclusive.Year, endExclusive.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        }
        return (start, endExclusive);
    }
    public static (DateTime start, DateTime endExclusive) GetDays(DateTime date, int days = 1)
    {
        var start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        var endExclusive = start + TimeSpan.FromDays(days);
        return (start, endExclusive);
    }

    public static (DateTime start, DateTime endExclusive) GetYear(DateTime date)
    {
        var start = new DateTime(date.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endExclusive = new DateTime(date.Year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (start, endExclusive);
    }
}
