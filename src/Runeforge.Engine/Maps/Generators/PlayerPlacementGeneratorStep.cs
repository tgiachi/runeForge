using Runeforge.Core.Extensions.Rnd;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Primitives.GridViews;
using Serilog;

namespace Runeforge.Engine.Maps.Generators;

public class PlayerPlacementGeneratorStep : IMapGeneratorStep
{
    private readonly IPlayerService _playerService;

    private readonly ILogger _logger = Log.ForContext<PlayerPlacementGeneratorStep>();

    public PlayerPlacementGeneratorStep(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        // For now, we will just place the player at a random position

        var emptyPositions = context.Map.Terrain.Positions().Where(s => context.Map.GetTerrainAt(s).IsWalkable).RandomItem();

        _logger.Information("Setting player position to {Position}", emptyPositions);


        _playerService.CreatePlayer();


        context.Map.AddEntity(_playerService.Player);

        return context;
    }
}
