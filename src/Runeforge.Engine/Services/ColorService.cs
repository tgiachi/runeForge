using Runeforge.Data.Json.Converters;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Primitives;
using Serilog;

namespace Runeforge.Engine.Services;

public record ColorObject(string Name, Color Color);

public class ColorService : IColorService
{
    private readonly ILogger _logger = Log.Logger.ForContext<ColorService>();

    private readonly Dictionary<string, List<ColorObject>> _colors = new();

    private string _defaultColorSet = "Default";

    public void AddColor(string colorSet, string colorName, Color color)
    {
        if (string.IsNullOrWhiteSpace(colorSet))
        {
            _logger.Error("Color set name cannot be null or empty.");
            throw new ArgumentException("Color set name cannot be null or empty.", nameof(colorSet));
        }

        if (string.IsNullOrWhiteSpace(colorName))
        {
            _logger.Error("Color name cannot be null or empty.");
            throw new ArgumentException("Color name cannot be null or empty.", nameof(colorName));
        }

        if (!_colors.TryGetValue(colorSet, out _))
        {
            _colors[colorSet] = [];
        }

        var colorObject = new ColorObject(colorName, color);
        _colors[colorSet].Add(colorObject);
        _logger.Information("Added color '{ColorName}' to color set '{ColorSet}'.", colorName, colorSet);
    }

    /// <summary>
    /// Gets a color by name with support for different formats:
    /// - "#RRGGBB" or "#AARRGGBB": Direct hex color
    /// - "colorset#colorname": Specific color from specific set
    /// - "colorname": Color from current color set, or default color set if current is not set
    /// </summary>
    /// <param name="colorName">The color identifier</param>
    /// <returns>The resolved color</returns>
    /// <exception cref="InvalidOperationException">When current color set is not set and no colorset is specified</exception>
    /// <exception cref="KeyNotFoundException">When the specified color or colorset is not found</exception>
    public Color GetColor(string colorName)
    {
        if (string.IsNullOrWhiteSpace(colorName))
        {
            _logger.Error("Color name cannot be null or empty.");
            throw new ArgumentException("Color name cannot be null or empty.", nameof(colorName));
        }

        // Case 1: Direct hex color (#RRGGBB or #AARRGGBB)
        if (colorName.StartsWith('#'))
        {
            var colorDef = HexColorConverter.ParseHexColor(colorName);
            return new Color(colorDef.R, colorDef.G, colorDef.B, colorDef.A);
        }

        // Case 2: Colorset#colorname format
        if (colorName.Contains('#'))
        {
            var parts = colorName.Split('#', 2);
            var targetColorSet = parts[0].Trim();
            var targetColorName = parts[1].Trim();

            if (string.IsNullOrWhiteSpace(targetColorSet) || string.IsNullOrWhiteSpace(targetColorName))
            {
                _logger.Error("Invalid color format: '{ColorName}'. Expected format: 'colorset#colorname'.", colorName);
                throw new ArgumentException($"Invalid color format: '{colorName}'. Expected format: 'colorset#colorname'.");
            }

            if (!_colors.TryGetValue(targetColorSet, out var colorList))
            {
                _logger.Error("Color set '{ColorSet}' does not exist.", targetColorSet);
                throw new KeyNotFoundException($"Color set '{targetColorSet}' does not exist.");
            }

            var colorObject = colorList.FirstOrDefault(c => c.Name.Equals(
                    targetColorName,
                    StringComparison.OrdinalIgnoreCase
                )
            );
            if (colorObject == null)
            {
                _logger.Error("Color '{ColorName}' not found in color set '{ColorSet}'.", targetColorName, targetColorSet);
                throw new KeyNotFoundException($"Color '{targetColorName}' not found in color set '{targetColorSet}'.");
            }

            return colorObject.Color;
        }

        // Case 3: Simple color name using default color set

        if (!_colors.TryGetValue(_defaultColorSet, out var currentColorList))
        {
            _logger.Error("Color set '{ColorSet}' does not exist.", _defaultColorSet);
            throw new KeyNotFoundException($"Color set '{_defaultColorSet}' does not exist.");
        }

        var currentColorObject =
            currentColorList.FirstOrDefault(c => c.Name.Equals(colorName, StringComparison.OrdinalIgnoreCase));
        if (currentColorObject == null)
        {
            _logger.Error("Color '{ColorName}' not found in color set '{ColorSet}'.", colorName, _defaultColorSet);
            throw new KeyNotFoundException($"Color '{colorName}' not found in color set '{_defaultColorSet}'.");
        }

        return currentColorObject.Color;
    }

    public void SetDefaultColorSet(string colorSet)
    {
        _defaultColorSet = colorSet;
    }
}
