using JsonReaderOptions = System.Text.Json.JsonReaderOptions;
using JsonWriterOptions = System.Text.Json.JsonWriterOptions;
using Utf8JsonReader = System.Text.Json.Utf8JsonReader;
using Utf8JsonWriter = System.Text.Json.Utf8JsonWriter;

public delegate void SerializeJsonAction<T>(Utf8JsonWriter writer, T? model) where T : notnull;

public delegate void DeserializeJsonAction<T>(ref Utf8JsonReader reader, T model) where T : notnull;

public static class ExtensionMethods
{
    public static Boolean TryDeserializeJson<T>(this BinaryData binaryData, ref T model, DeserializeJsonAction<T> action, JsonReaderOptions options = default) where T : notnull
    {
        try
        {
            var reader = new Utf8JsonReader(binaryData, options);
            if (!reader.Read()) { return false; }
            action(ref reader, model);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
