using System.Text.RegularExpressions;
using Runeforge.Engine.Services;

namespace Runeforge.Engine.Utils;

/// <summary>
/// Static class for parsing symbol strings with random support
/// </summary>
public static partial class SymbolParser
{
    [GeneratedRegex(@"rnd\(([^)]+)\)", RegexOptions.IgnoreCase)]
    private static partial Regex RandomRegex();

    /// <summary>
    /// Parses a symbol string and returns the appropriate type
    /// Supports:
    /// - Single character: returns as char
    /// - Single number: returns as int
    /// - rnd(1-10): random number between 1 and 10
    /// - rnd(1,2,3,4): random choice from the list
    /// </summary>
    /// <typeparam name="T">Target type (char, int, string)</typeparam>
    /// <param name="symbol">Symbol string to parse</param>
    /// <returns>Parsed value of type T</returns>
    /// <exception cref="ArgumentException">When symbol format is invalid</exception>
    /// <exception cref="InvalidOperationException">When conversion to target type fails</exception>
    public static T ParseSymbol<T>(string symbol)
    {
        if (string.IsNullOrWhiteSpace(symbol))
        {
            throw new ArgumentException("Symbol cannot be null or empty", nameof(symbol));
        }

        symbol = symbol.Trim();

        // Check if it contains random pattern
        var randomMatch = RandomRegex().Match(symbol);
        if (randomMatch.Success)
        {
            var randomContent = randomMatch.Groups[1].Value.Trim();
            var randomResult = ParseRandomExpression(randomContent);
            return ConvertToType<T>(randomResult);
        }

        // Single character
        if (symbol.Length == 1)
        {
            if (typeof(T) == typeof(char))
            {
                return (T)(object)symbol[0];
            }

            // Try to parse as number if it's a digit
            if (char.IsDigit(symbol[0]))
            {
                var digit = int.Parse(symbol);
                return ConvertToType<T>(digit);
            }

            // Return as character converted to target type
            return ConvertToType<T>(symbol[0]);
        }

        // Try to parse as integer
        if (int.TryParse(symbol, out var intValue))
        {
            return ConvertToType<T>(intValue);
        }

        // Return as string converted to target type
        return ConvertToType<T>(symbol);
    }

    /// <summary>
    /// Parses random expressions like "1-10" or "1,2,3,4"
    /// </summary>
    /// <param name="randomContent">Content inside rnd() parentheses</param>
    /// <returns>Random result as object</returns>
    private static object ParseRandomExpression(string randomContent)
    {
        // Range format: "1-10"
        if (randomContent.Contains('-'))
        {
            var parts = randomContent.Split('-', 2);
            if (parts.Length == 2 &&
                int.TryParse(parts[0].Trim(), out var min) &&
                int.TryParse(parts[1].Trim(), out var max))
            {
                return Random.Shared.Next(min, max + 1);
            }

            throw new ArgumentException($"Invalid range format: {randomContent}");
        }

        // List format: "1,2,3,4" or "a,b,c,d"
        if (randomContent.Contains(','))
        {
            var items = randomContent.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .ToArray();

            if (items.Length == 0)
            {
                throw new ArgumentException("Empty random list");
            }

            var randomItem = items[Random.Shared.Next(items.Length)];

            // Try to parse as integer first
            if (int.TryParse(randomItem, out var intValue))
            {
                return intValue;
            }

            // Single character
            if (randomItem.Length == 1)
            {
                return randomItem[0];
            }

            // Return as string
            return randomItem;
        }

        // Single value - shouldn't really happen in rnd() but handle it
        if (int.TryParse(randomContent, out var singleInt))
        {
            return singleInt;
        }

        if (randomContent.Length == 1)
        {
            return randomContent[0];
        }

        return randomContent;
    }

    /// <summary>
    /// Converts an object to the specified type
    /// </summary>
    /// <typeparam name="T">Target type</typeparam>
    /// <param name="value">Value to convert</param>
    /// <returns>Converted value</returns>
    /// <exception cref="InvalidOperationException">When conversion is not possible</exception>
    private static T ConvertToType<T>(object value)
    {
        try
        {
            if (value is T directValue)
            {
                return directValue;
            }

            // Handle char conversions
            if (typeof(T) == typeof(char))
            {
                return value switch
                {
                    char c => (T)(object)c,
                    int i when i >= 0 && i <= 65535 => (T)(object)(char)i,
                    string s when s.Length == 1 => (T)(object)s[0],
                    _ => throw new InvalidOperationException($"Cannot convert {value} ({value.GetType()}) to char")
                };
            }

            // Handle int conversions
            if (typeof(T) == typeof(int))
            {
                return value switch
                {
                    int i => (T)(object)i,
                    char c => (T)(object)(int)c,
                    string s when int.TryParse(s, out var parsed) => (T)(object)parsed,
                    _ => throw new InvalidOperationException($"Cannot convert {value} ({value.GetType()}) to int")
                };
            }

            // Handle string conversions
            if (typeof(T) == typeof(string))
            {
                return (T)(object)value.ToString();
            }

            // Try generic conversion
            return (T)Convert.ChangeType(value, typeof(T));
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Cannot convert {value} ({value.GetType()}) to {typeof(T)}", ex);
        }
    }

    /// <summary>
    /// Parses a TileDataObj symbol and returns the resolved symbol
    /// </summary>
    /// <param name="tileData">Tile data object</param>
    /// <returns>Resolved symbol as char</returns>
    public static char ParseTileSymbol(TileDataObj tileData)
    {
        return ParseSymbol<char>(tileData.Symbol);
    }

    /// <summary>
    /// Parses a TileDataObj symbol and returns the resolved symbol as integer (glyph code)
    /// </summary>
    /// <param name="tileData">Tile data object</param>
    /// <returns>Resolved symbol as integer glyph code</returns>
    public static int ParseTileSymbolAsGlyph(TileDataObj tileData)
    {
        return ParseSymbol<int>(tileData.Symbol);
    }
}
