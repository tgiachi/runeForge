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

    private readonly Dictionary<string, AnimationData> _animations = new();

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
            tileData.Tags?.ToArray(),
            tileData.AnimationId
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

    public void AddAnimation(string tileSet, JsonTileAnimationData animationData)
    {
        if (animationData == null)
        {
            _logger.Warning("Attempted to add a null animation to tileset {TileSet}", tileSet);
            return;
        }

        var animation = new AnimationData(animationData);
        _animations[animation.Id] = animation;

        _logger.Debug("Added animation {AnimationId} to tileset {TileSet}", animation.Id, tileSet);
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

        AnimationData animationData = null;

        if (!string.IsNullOrEmpty(tileDataObj.AnimationId))
        {
            animationData = _animations[tileDataObj.AnimationId];
        }

        return new TileColoredGlyph(tileDataObj, animationData);
    }

    public TileColoredGlyph CreateGlyph(JsonHasTile tileData)
    {
        if (tileData.Symbol.Length > 1)
        {
            return CreateGlyph(tileData.Symbol);
        }

        var coloredGlyph = new ColoredGlyph(
            _colorService.GetColor(tileData.Foreground),
            _colorService.GetColor(tileData.Background),
            SymbolParser.ParseSymbol<int>(tileData.Symbol)
        );

        return new TileColoredGlyph(coloredGlyph, true, false, null);
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
    string[] Tags = null,
    string AnimationId = null
);

public record TileColoredGlyph(ColoredGlyph ColoredGlyph, bool IsBlocking, bool IsTransparent, AnimationData? Animation)
{
    public TileColoredGlyph(TileDataObj tileData, AnimationData animation)
        : this(
            new ColoredGlyph(tileData.Foreground, tileData.Background, SymbolParser.ParseTileSymbolAsGlyph(tileData)),
            tileData.IsBlocking,
            tileData.IsTransparent,
            animation
        )
    {
    }
}

public record AnimationData(
    string Id,
    string[] Frames,
    bool Loop,
    int Interval,
    string? StartForeground = null,
    string? EndForeground = null,
    string? StartBackground = null,
    string? EndBackground = null
)
{
    public AnimationData(JsonTileAnimationData animation)
        : this(
            animation.Id,
            animation.Frames.ToArray(),
            animation.Loop,
            animation.Duration,
            animation.Foreground?.Start,
            animation.Foreground?.End,
            animation.Background?.Start,
            animation.Background?.End
        )
    {
    }
};
