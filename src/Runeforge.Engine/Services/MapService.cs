using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using Runeforge.Data.Types.Map;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Integration;

namespace Runeforge.Engine.Services;

public class MapService : IMapService
{
    public event IMapService.MapEntityAddedHandler<RogueLikeEntity>? EntityAdded;
    public event IMapService.MapEntityAddedHandler<RogueLikeEntity>? EntityRemoved;
    public event IMapService.MapEntityAddedHandler<NpcGameObject>? NpcAdded;
    public event IMapService.MapEntityAddedHandler<NpcGameObject>? NpcRemoved;
    public event IMapService.MapEntityAddedHandler<ItemGameObject>? ItemAdded;
    public event IMapService.MapEntityAddedHandler<ItemGameObject>? ItemRemoved;
    public event IMapService.MapGeneratedHandler? MapGenerated;
    public event IMapService.MapChangedHandler? MapChanged;
    public event IMapService.MapStartGeneratedHandler? MapStartGenerated;

    private readonly IMapGeneratorService _mapGeneratorService;

    public MapInfoObject CurrentMap
    {
        get => _currentMap;
        set => ChangeCurrentMap(value);
    }

    private MapInfoObject _currentMap = null!;

    private readonly Dictionary<string, MapInfoObject> _maps = new();

    public MapService(IMapGeneratorService mapGeneratorService)
    {
        _mapGeneratorService = mapGeneratorService;
    }

    private void ChangeCurrentMap(MapInfoObject newMap)
    {
        var oldMap = _currentMap;

        if (oldMap != null)
        {
            DeInitEntitiesHandlers();
        }

        _currentMap = newMap;

        InitEntitiesHandlers();

        MapChanged?.Invoke(oldMap, newMap);
    }


    private void InitEntitiesHandlers()
    {
        CurrentMap.Map.EntityAdded += MapOnEntityAdded;
    }

    private void MapOnEntityAdded(IGameObject gameObject, MapLayer layer)
    {
    }

    private void DeInitEntitiesHandlers()
    {
        CurrentMap.Map.EntityAdded -= MapOnEntityAdded;
    }


    public async Task<string> GenerateMapAsync(
        int width, int height, string name, string description, int level = 1, CancellationToken cancellationToken = default
    )
    {
        var mapId = Guid.NewGuid().ToString();

        MapStartGenerated?.Invoke(mapId);

        var generator = new Generator(width, height)
            .ConfigAndGenerateSafe(c => c.AddSteps(DefaultAlgorithms.RectangleMapSteps()))
            .Generate();

        var gameMap = new GameMap(width, height, null);

        gameMap.AllComponents.Add(new TerrainFOVVisibilityHandler());

        var mapInfo = new MapInfoObject(gameMap, name, description, level);

        await MapGenerated?.Invoke(mapInfo);

        _maps[mapId] = mapInfo;

        CurrentMap = mapInfo;


        return mapId;
    }

    public async Task StartGenerateMapAsync(string generatorName, string mapId = "")
    {
        if (string.IsNullOrWhiteSpace(mapId))
        {
            mapId = Guid.NewGuid().ToString();
        }

        MapStartGenerated?.Invoke(mapId);

        var map = await _mapGeneratorService.ExecuteGenerationAsync(generatorName, mapId);

        var mapInfo = new MapInfoObject(map, map.Name, map.Description, map.Level);

        _maps[mapId] = mapInfo;
        MapGenerated?.Invoke(mapInfo);
    }

    public MapInfoObject? GetMapInfo(string mapId)
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

        if (entity is NpcGameObject npc)
        {
            NpcAdded?.Invoke(npc);
        }

        if (entity is ItemGameObject item)
        {
            ItemAdded?.Invoke(item);
        }

        EntityAdded?.Invoke(entity);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
    }
}
