using Runeforge.Data.Entities.Tileset;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Primitives;
using Serilog;

namespace Runeforge.Engine.Services;

public class TileSetService : ITileSetService
{
    private readonly ILogger _logger = Log.ForContext<TileSetService>();
    private readonly IColorService _colorService;

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
            _tilesets[tileSet] = [];
        }

        var tileDataObj = ToTileData(tileData);
        value.Add(tileDataObj);

        _logger.Debug("Added tile {TileId} to tileset {TileSet}", tileData.Id, tileSet);
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
