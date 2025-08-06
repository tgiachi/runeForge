using System.ComponentModel;
using GoRogue.MapGeneration;
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

    public MapInfoObject CurrentMap
    {
        get => _currentMap;
        set => ChangeCurrentMap(value);
    }

    private MapInfoObject _currentMap = null!;

    private readonly Dictionary<Guid, MapInfoObject> _maps = new();

    private void ChangeCurrentMap(MapInfoObject newMap)
    {
        var oldMap = _currentMap;
        _currentMap = newMap;

        MapChanged?.Invoke(oldMap, newMap);
    }


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
