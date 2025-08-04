using Runeforge.Engine.Contexts;

namespace Runeforge.Engine.Interfaces.Maps;

public interface IMapGeneratorStep
{
    Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context);
}
