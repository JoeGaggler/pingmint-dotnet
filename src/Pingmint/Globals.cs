using System.Security.Cryptography;

namespace Pingmint;

/// <summary>
/// Shared global methods
/// </summary>
public static partial class Globals
{
    /// <summary>
    /// Generates a random alpha-numeric string of the specified length.
    /// </summary>
    /// <param name="length">Length of string to return</param>
    /// <returns>Random string</returns>
    public static String RandomAlphaNumericString(int length) => RandomNumberGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", length);

    /// <summary>
    /// Generates a random Base 64 URL string of the specified length.
    /// </summary>
    /// <param name="length">Length of string to return</param>
    /// <returns>Random string</returns>
    public static String RandomBase64UrlString(int length) => RandomNumberGenerator.GetString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_", length);
}
