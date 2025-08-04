using MoonSharp.Interpreter;
using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Interfaces.Services;
using SadConsole;

namespace Runeforge.Engine.Modules;

[ScriptModule("tiles")]
public class TilesModule
{
    private readonly ITileSetService _tileSetService;

    public TilesModule(ITileSetService tileSetService)
    {
        _tileSetService = tileSetService;
    }


    [ScriptFunction("create tile from tag or tileId")]
    public ColoredGlyph Create(string tileOrTag)
    {
        var tile = _tileSetService.CreateGlyph(tileOrTag);
        if (tile == null)
        {
            throw new ScriptRuntimeException($"Tile '{tileOrTag}' not found in tile set.");
        }

        return tile.ColoredGlyph;
    }
}
