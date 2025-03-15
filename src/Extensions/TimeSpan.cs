namespace Pingmint;

/// <summary>
/// Extension methods for the <see cref="TimeSpan"/> class.
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Gets a short string representation of a <see cref="TimeSpan"/> value.
    /// </summary>
    /// <param name="value"><see cref="TimeSpan"/> value</param>
    /// <returns>String representation of the <see cref="TimeSpan"/></returns>
    public static String ToShortString(this TimeSpan value)
    {
        if (value < TimeSpan.Zero) { value = -value; }
        if (value.TotalMinutes < 1) { return $"{(int)value.TotalSeconds:0}s"; }
        if (value.TotalHours < 1) { return $"{(int)value.TotalMinutes:0}m{value.Seconds:0}s"; }
        if (value.TotalDays < 1) { return $"{(int)value.TotalHours:0}h{value.Minutes:0}m{value.Seconds:0}s"; }
        return $"{(int)value.TotalDays:0}d{value.Hours:0}h{(int)value.Minutes:0}m{value.Seconds:0}s";
    }
}
