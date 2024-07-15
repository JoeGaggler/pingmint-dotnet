using System.Text.Json;

namespace Pingmint;

public delegate void DeserializeJsonAction<T>(ref Utf8JsonReader reader, T model) where T : class;

public sealed class Json
{
    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(ReadOnlySpan<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new()
    {
        try
        {
            var reader = new Utf8JsonReader(data, options);
            if (!reader.Read())
            {
                return default;
            }

            T model = new();
            action(ref reader, model);
            return model;
        }
        catch
        {
            return default;
        }
    }

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(Span<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize(data, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(String data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize(data.ToUtf8Bytes().AsSpan(), action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(ReadOnlyMemory<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize(data.Span, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(BinaryData data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize(data, action, options);
}

/// <summary>
/// Deserialize extension methods for a subset of methods from <see cref="Json"/>.
/// </summary>
public static class JsonDeserializerExtensions
{
    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this ReadOnlySpan<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Json.Deserialize(data, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this Span<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Json.Deserialize(data, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this ReadOnlyMemory<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Json.Deserialize(data.Span, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this BinaryData data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Json.Deserialize(data, action, options);
}
