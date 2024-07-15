using System.Text.Json;

namespace Pingmint;

public delegate void DeserializeJsonAction<T>(ref Utf8JsonReader reader, T model) where T : class;

public static class ForJsonCodeGen
{
    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this ReadOnlySpan<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new()
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
    public static T? DeserializeJson<T>(this Span<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        DeserializeJson(data, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this String data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        DeserializeJson(data.ToUtf8Bytes().AsSpan(), action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this ReadOnlyMemory<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        DeserializeJson(data.Span, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? DeserializeJson<T>(this BinaryData data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        DeserializeJson(data, action, options);
}
