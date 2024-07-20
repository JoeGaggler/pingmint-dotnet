namespace Pingmint;

public static class TimeSpanExtensions
{
    public static String ToShortString(this TimeSpan value)
    {
        if (value < TimeSpan.Zero) { value = -value; }
        if (value.TotalMinutes < 1) { return $"{(int)value.TotalSeconds:0}s"; }
        if (value.TotalHours < 1) { return $"{(int)value.TotalMinutes:0}m{value.Seconds:0}s"; }
        if (value.TotalDays < 1) { return $"{(int)value.TotalHours:0}h{value.Minutes:0}m{value.Seconds:0}s"; }
        return $"{(int)value.TotalDays:0}d{value.Hours:0}h{(int)value.Minutes:0}m{value.Seconds:0}s";
    }
}
