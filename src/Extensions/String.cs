namespace Pingmint;

public static class StringExtensions
{
    /// <summary>
    /// Gets the UTF-8 bytes of a string.
    /// </summary>
    /// <param name="value">Instance from which to obtain the byte array</param>
    /// <returns>UTF8 byte array</returns>
    public static Byte[] ToUtf8Bytes(this String value) => System.Text.Encoding.UTF8.GetBytes(value);
}
