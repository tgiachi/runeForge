using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Maps;

namespace Runeforge.Engine.Maps.Generators;

public class RectangleMapGeneratorStep : IMapGeneratorStep
{
    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        return context;
    }
}
