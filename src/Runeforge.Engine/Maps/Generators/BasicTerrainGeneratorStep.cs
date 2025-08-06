using System.Text.Json;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services;
using SadConsole;
using SadRogue.Primitives.GridViews;

namespace Runeforge.Engine.Maps.Generators;

public class BasicTerrainGeneratorStep : IMapGeneratorStep
{
    private readonly ITileSetService _tileSetService;

    public BasicTerrainGeneratorStep(ITileSetService tileSetService)
    {
        _tileSetService = tileSetService;
    }

    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        var terrain = context.GetOutput("terrain") as ISettableGridView<bool>;

        var wallTile = ((JsonElement)context.Inputs["wall"]).GetString();
        var floorTile = ((JsonElement)context.Inputs["floor"]).GetString();


        context.Map.ApplyTerrainOverlay(
            terrain,
            (point, val) => val
                ? new TerrainGameObject(point, CreateColoredGlyph(floorTile).Item1, "floor")
                : new TerrainGameObject(point, CreateColoredGlyph(wallTile).Item1, "wall", false)
        );

        return context;
    }

    private (ColoredGlyph, string) CreateColoredGlyph(string tile)
    {
        var glyph = _tileSetService.CreateGlyph(tile);

        return (glyph.ColoredGlyph, glyph.TileId);
    }
}
