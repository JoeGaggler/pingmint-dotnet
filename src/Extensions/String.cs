namespace Pingmint;

/// <summary>
/// Extension methods for the <see cref="String"/> class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Gets the UTF-8 bytes of a string.
    /// </summary>
    /// <param name="value">String of UTF-8 characters</param>
    /// <returns>UTF-8 byte array</returns>
    public static Byte[] ToUtf8Bytes(this String value) => System.Text.Encoding.UTF8.GetBytes(value);

    /// <summary>
    /// Gets the UTF-8 string of a byte array.
    /// </summary>
    /// <param name="value">Byte array of UTF-8 characters</param>
    /// <returns>String of UTF-8 characters</returns>
    public static String ToUtf8String(this Byte[] value) => System.Text.Encoding.UTF8.GetString(value);
}
