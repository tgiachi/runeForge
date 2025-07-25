using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using Runeforge.Data.Colors;

namespace Runeforge.Data.Json.Converters;

/// <summary>
/// JSON converter that transforms hex color strings (#RRGGBB or #AARRGGBB) into XNA Color objects
/// </summary>
public class HexColorConverter : JsonConverter<ColorDef>
{
    public override ColorDef Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string token for color, got {reader.TokenType}");
        }

        string hexValue = reader.GetString();

        if (string.IsNullOrEmpty(hexValue))
        {
            throw new JsonException("Color hex value cannot be null or empty");
        }

        return ParseHexColor(hexValue);
    }

    public override void Write(Utf8JsonWriter writer, ColorDef value, JsonSerializerOptions options)
    {
        /// Convert Color back to hex string for serialization
        string hexString = $"#{value.R:X2}{value.G:X2}{value.B:X2}";

        /// Include alpha if it's not fully opaque
        if (value.A != 255)
        {
            hexString = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
        }

        writer.WriteStringValue(hexString);
    }

    /// <summary>
    /// Parse hex color string into XNA Color object
    /// Supports formats: #RGB, #RRGGBB, #AARRGGBB
    /// </summary>
    private static ColorDef ParseHexColor(string hexValue)
    {
        if (!hexValue.StartsWith("#"))
        {
            throw new JsonException($"Color hex value must start with '#': {hexValue}");
        }

        string hex = hexValue.Substring(1);

        try
        {
            switch (hex.Length)
            {
                case 3: /// #RGB format
                    return ParseRgb3(hex);

                case 6: /// #RRGGBB format
                    return ParseRgb6(hex);

                case 8: /// #AARRGGBB format
                    return ParseArgb8(hex);

                default:
                    throw new JsonException($"Invalid hex color format: {hexValue}. Expected #RGB, #RRGGBB, or #AARRGGBB");
            }
        }
        catch (FormatException ex)
        {
            throw new JsonException($"Invalid hex color value: {hexValue}", ex);
        }
        catch (OverflowException ex)
        {
            throw new JsonException($"Invalid hex color value: {hexValue}", ex);
        }
    }

    /// <summary>
    /// Parse 3-character hex (#RGB) - each character represents one component
    /// </summary>
    private static ColorDef ParseRgb3(string hex)
    {
        byte r = (byte)(Convert.ToByte(hex.Substring(0, 1), 16) * 17); /// F -> FF
        byte g = (byte)(Convert.ToByte(hex.Substring(1, 1), 16) * 17);
        byte b = (byte)(Convert.ToByte(hex.Substring(2, 1), 16) * 17);

        return new ColorDef(r, g, b);
    }

    /// <summary>
    /// Parse 6-character hex (#RRGGBB)
    /// </summary>
    private static ColorDef ParseRgb6(string hex)
    {
        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);

        return new ColorDef(r, g, b);
    }

    /// <summary>
    /// Parse 8-character hex (#AARRGGBB)
    /// </summary>
    private static ColorDef ParseArgb8(string hex)
    {
        byte a = Convert.ToByte(hex.Substring(0, 2), 16);
        byte r = Convert.ToByte(hex.Substring(2, 2), 16);
        byte g = Convert.ToByte(hex.Substring(4, 2), 16);
        byte b = Convert.ToByte(hex.Substring(6, 2), 16);

        return new ColorDef(r, g, b, a);
    }
}
