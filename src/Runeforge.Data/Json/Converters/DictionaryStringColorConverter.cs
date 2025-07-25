using System.Text.Json;
using System.Text.Json.Serialization;
using Runeforge.Data.Colors;

namespace Runeforge.Data.Json.Converters;

/// <summary>
/// Custom converter for Dictionary<string, Color> to handle hex color strings
/// </summary>
public class DictionaryStringColorConverter : JsonConverter<Dictionary<string, ColorDef>>
{
    private readonly HexColorConverter _colorConverter = new();

    public override Dictionary<string, ColorDef> Read(
        ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException($"Expected StartObject token, got {reader.TokenType}");
        }

        var dictionary = new Dictionary<string, ColorDef>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException($"Expected PropertyName token, got {reader.TokenType}");
            }

            string propertyName = reader.GetString()!;

            reader.Read(); /// Move to the value
            var color = _colorConverter.Read(ref reader, typeof(ColorDef), options);

            dictionary[propertyName] = color;
        }

        return dictionary;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, ColorDef> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key);
            _colorConverter.Write(writer, kvp.Value, options);
        }

        writer.WriteEndObject();
    }
}
