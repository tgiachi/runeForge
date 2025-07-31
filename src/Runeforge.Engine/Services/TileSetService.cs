using Runeforge.Data.Entities.Tileset;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Utils;
using SadConsole;
using SadRogue.Primitives;
using Serilog;

namespace Runeforge.Engine.Services;

public class TileSetService : ITileSetService
{
    private readonly ILogger _logger = Log.ForContext<TileSetService>();
    private readonly IColorService _colorService;

    private string _defaultTileSet;

    private readonly Dictionary<string, List<TileDataObj>> _tilesets = new();

    public TileSetService(IColorService colorService)
    {
        _colorService = colorService;
    }

    private TileDataObj ToTileData(JsonTileData tileData)
    {
        return new TileDataObj(
            tileData.Id,
            tileData.Symbol,
            tileData.IsBlocking,
            tileData.IsTransparent,
            _colorService.GetColor(tileData.Foreground),
            _colorService.GetColor(tileData.Background),
            tileData.Tags?.ToArray()
        );
    }

    public void AddTile(string tileSet, JsonTileData tileData)
    {
        if (!_tilesets.TryGetValue(tileSet, out List<TileDataObj>? value))
        {
            value = new List<TileDataObj>();
            _tilesets[tileSet] = value;
        }

        var tileDataObj = ToTileData(tileData);
        value.Add(tileDataObj);

        _logger.Debug("Added tile {TileId} to tileset {TileSet}", tileData.Id, tileSet);
    }

    public TileColoredGlyph CreateGlyph(string nameOrTag)
    {
        TileDataObj tileDataObj = null;

        foreach (var tileset in _tilesets.Values)
        {
            tileDataObj = tileset.FirstOrDefault(t => t.Id == nameOrTag || (t.Tags != null && t.Tags.Contains(nameOrTag)));
            if (tileDataObj != null)
            {
                break;
            }
        }

        if (tileDataObj == null)
        {
            throw new KeyNotFoundException($"Tile with name or tag '{nameOrTag}' not found in any tileset.");
        }

        return new TileColoredGlyph(tileDataObj);
    }

    public void SetDefaultTileSet(string tileSetId)
    {
        _defaultTileSet = tileSetId;
    }

    public string GetDefaultTileSet()
    {
        return _defaultTileSet;
    }
}

public record TileDataObj(
    string Id,
    string Symbol,
    bool IsBlocking,
    bool IsTransparent,
    Color Foreground,
    Color Background,
    string[] Tags = null
);

public record TileColoredGlyph(ColoredGlyph coloredGlyph, bool IsBlocking, bool IsTransparent)
{
    public TileColoredGlyph(TileDataObj tileData)
        : this(
            new ColoredGlyph(tileData.Foreground, tileData.Background, SymbolParser.ParseTileSymbolAsGlyph(tileData)),
            tileData.IsBlocking,
            tileData.IsTransparent
        )
    {
    }
}
