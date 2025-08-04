using GoRogue.MapGeneration;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Maps;
using SadRogue.Primitives.GridViews;

namespace Runeforge.Engine.Maps.Generators;

public class RectangleMapGeneratorStep : IMapGeneratorStep
{
    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        var generator =
            new Generator(context.Width, context.Height).ConfigAndGenerateSafe(s =>
                s.AddSteps(DefaultAlgorithms.RectangleMapSteps())
            );

        var terrainGrid = generator.Context.GetFirstOrDefault<ISettableGridView<bool>>("WallFloor");



        context.AddOutput("TerrainGrid", terrainGrid);




        return context;
    }
}
