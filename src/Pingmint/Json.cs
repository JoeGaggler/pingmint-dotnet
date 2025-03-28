using System.Text.Json;

namespace Pingmint;

/// <summary>
/// Delegate for serializing JSON.
/// </summary>
/// <typeparam name="T">Model to serialize</typeparam>
/// <param name="writer">Serialization output</param>
/// <param name="model">Model to serialize</param>
public delegate void SerializeJsonAction<T>(Utf8JsonWriter writer, T model) where T : class;

/// <summary>
/// Delegate for deserializing JSON.
/// </summary>
/// <typeparam name="T">Model to deserialize</typeparam>
/// <param name="reader">Deserialization input</param>
/// <param name="model">Model to deserialize</param>
public delegate void DeserializeJsonAction<T>(ref Utf8JsonReader reader, T model) where T : class;

/// <summary>
/// Methods for JSON serialization.
/// </summary>
public sealed class Json
{
    /// <summary>
    /// Serialize an object to JSON using a delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON serializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonWriter"/></param>
    /// <returns>Serialized JSON.</returns>
    public static ReadOnlySpan<byte> SerializeToUtf8Span<T>(T data, SerializeJsonAction<T> action, JsonWriterOptions options = default) where T : class
    {
        var buffer = new System.Buffers.ArrayBufferWriter<Byte>();
        using (var writer = new Utf8JsonWriter(buffer, options))
        {
            action(writer, data);
        }
        return buffer.WrittenSpan;
    }

    /// <summary>
    /// Serialize an object to JSON string using a delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON serializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonWriter"/></param>
    /// <returns>Serialized JSON.</returns>
    public static ReadOnlyMemory<Byte> SerializeToUtf8Memory<T>(T data, SerializeJsonAction<T> action, JsonWriterOptions options = default) where T : class
    {
        var buffer = new System.Buffers.ArrayBufferWriter<Byte>();
        using (var writer = new Utf8JsonWriter(buffer, options))
        {
            action(writer, data);
        }
        return buffer.WrittenMemory;
    }

    /// <summary>
    /// Serialize an object to JSON string using a delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON serializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonWriter"/></param>
    /// <returns>Serialized JSON.</returns>
    public static String SerializeToString<T>(T data, SerializeJsonAction<T> action, JsonWriterOptions options = default) where T : class
    {
        return System.Text.Encoding.UTF8.GetString(SerializeToUtf8Span(data, action, options));
    }

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
    public static T? Deserialize<T>(Byte[] data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize((ReadOnlySpan<Byte>)data, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(Span<Byte> data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize((ReadOnlySpan<Byte>)data, action, options);

    /// <summary>
    /// Deserialize a JSON object using an delegate.
    /// </summary>
    /// <typeparam name="T">Type represented in JSON</typeparam>
    /// <param name="data">JSON payload</param>
    /// <param name="action">JSON deserializer delegate</param>
    /// <param name="options">Options for <see cref="Utf8JsonReader"/></param>
    /// <returns>Deserialized object, or null on error.</returns>
    public static T? Deserialize<T>(String data, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : class, new() =>
        Deserialize(new ReadOnlySpan<Byte>(data.ToUtf8Bytes()), action, options);

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
}
