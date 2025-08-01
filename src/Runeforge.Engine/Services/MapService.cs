using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Integration;

namespace Runeforge.Engine.Services;

public class MapService : IMapService
{
    public event IMapService.MapGeneratedHandler? MapGenerated;
    public event IMapService.MapStartGeneratedHandler? MapStartGenerated;
    public MapInfoObject CurrentMap { get; set; }

    private readonly Dictionary<Guid, MapInfoObject> _maps = new();

    public async Task<Guid> GenerateMapAsync(
        int width, int height, string name, string description, int level = 1, CancellationToken cancellationToken = default
    )
    {
        var mapId = Guid.NewGuid();

        MapStartGenerated?.Invoke(mapId);

        var generator = new Generator(width, height)
            .ConfigAndGenerateSafe(c => c.AddSteps(DefaultAlgorithms.RectangleMapSteps()))
            .Generate();

        var gameMap = new GameMap(width, height, null);

        gameMap.AllComponents.Add(new TerrainFOVVisibilityHandler());

        var mapInfo = new MapInfoObject(gameMap, name, description, level);

        await MapGenerated?.Invoke(mapInfo, generator);

        _maps[mapId] = mapInfo;

        CurrentMap = mapInfo;


        return mapId;
    }

    public MapInfoObject? GetMapInfo(Guid mapId)
    {
        return _maps.GetValueOrDefault(mapId);
    }

    public void AddEntityInCurrentMap<TEntity>(TEntity entity) where TEntity : RogueLikeEntity
    {
        if (CurrentMap == null)
        {
            throw new InvalidOperationException("Current map is not set.");
        }

        CurrentMap.Map.AddEntity(entity);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var generator = new Generator(300, 300)
            .ConfigAndGenerateSafe(c => c.AddComponent(DefaultAlgorithms.RectangleMapSteps()))
            .Generate();
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
    }
}
