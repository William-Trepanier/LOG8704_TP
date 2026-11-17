using System;

public class TimeInfo
{
    public readonly double seconds;
    public readonly string label;
        
    public TimeInfo(double seconds)
    {
        this.seconds = seconds;
        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(seconds));
        label = dateTimeOffset.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
    }

    public TimeInfo(double startTime, double duration)
    {
        var start = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(startTime));
        var end = start.AddSeconds(duration);
        
        var totalMonths = (end.Year - start.Year) * 12 + (end.Month - start.Month);
        if (start.AddMonths(totalMonths) > end) totalMonths--;
        
        var years = totalMonths / 12;
        var months = totalMonths % 12;
        var remaining = end - start.AddMonths(totalMonths);

        seconds = end.ToUnixTimeSeconds() - start.ToUnixTimeSeconds();
        label =
            $"{years}-{months}-{remaining.Days} {remaining.Hours}:{remaining.Minutes}:{remaining.Seconds}";
    }

    public TimeInfo()
    {
        seconds = double.PositiveInfinity;
        label = "∞";
    }
}