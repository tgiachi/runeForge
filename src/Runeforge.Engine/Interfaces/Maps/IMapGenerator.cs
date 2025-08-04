using Runeforge.Engine.Contexts;

namespace Runeforge.Engine.Interfaces.Maps;

public interface IMapGenerator
{
    Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context);
}
