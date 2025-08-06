using GoRogue.MapGeneration;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services.Base;
using SadRogue.Integration;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapService : IRuneforgeStartableService
{
    delegate Task MapGeneratedHandler(MapInfoObject mapInfo);

    delegate void MapStartGeneratedHandler(string id);

    delegate void MapChangedHandler(MapInfoObject OldMap, MapInfoObject NewMap);

    delegate void MapEntityAddedHandler<in TEntity>(TEntity entity) where TEntity : RogueLikeEntity;

    event MapEntityAddedHandler<RogueLikeEntity>? EntityAdded;
    event MapEntityAddedHandler<RogueLikeEntity>? EntityRemoved;

    event MapEntityAddedHandler<NpcGameObject>? NpcAdded;
    event MapEntityAddedHandler<NpcGameObject>? NpcRemoved;

    event MapEntityAddedHandler<ItemGameObject>? ItemAdded;

    event MapEntityAddedHandler<ItemGameObject>? ItemRemoved;

    event MapGeneratedHandler? MapGenerated;

    event MapChangedHandler? MapChanged;

    event MapStartGeneratedHandler? MapStartGenerated;


    MapInfoObject CurrentMap { get; set; }

    Task<string> GenerateMapAsync(
        int width, int height, string name, string description, int level = 1, CancellationToken cancellationToken = default
    );

    Task StartGenerateMapAsync(string generatorName, string mapId = "");



    MapInfoObject? GetMapInfo(string mapId);

    void AddEntityInCurrentMap<TEntity>(TEntity entity) where TEntity : RogueLikeEntity;
}
