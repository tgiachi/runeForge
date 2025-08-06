using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Maps.Generators;

public class PlayerPlacementGeneratorStep : IMapGeneratorStep
{

    private readonly INpcService _npcService;

    public PlayerPlacementGeneratorStep(INpcService npcService)
    {
        _npcService = npcService;
    }

    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        

        return context;
    }
}
