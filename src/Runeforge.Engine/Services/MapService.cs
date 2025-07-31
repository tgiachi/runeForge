using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using Runeforge.Engine.Data;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Types.Map;
using SadRogue.Integration.Maps;
using SadRogue.Primitives;

namespace Runeforge.Engine.Services;

public class MapService : IMapService
{
    public event IMapService.MapGeneratedHandler? MapGenerated;
    public event IMapService.MapStartGeneratedHandler? MapStartGenerated;
    public DefaultRendererParams DefaultRendererParams { get; set; }
    public MapInfoObject CurrentMap { get; set; }

    private readonly Dictionary<Guid, MapInfoObject> _maps = new();

    public async Task<Guid> GenerateMapAsync(
        int width, int height, string name, string description, int level = 1, CancellationToken cancellationToken = default
    )
    {
        var mapId = Guid.NewGuid();

        MapStartGenerated?.Invoke(mapId);

        var generator = new Generator(width, height)
            .ConfigAndGenerateSafe(c => c.AddComponent(DefaultAlgorithms.RectangleMapSteps()))
            .Generate();

        var gameMap = new GameMap(width, height, DefaultRendererParams);

        var mapInfo = new MapInfoObject(gameMap, name, description, level);

        await MapGenerated?.Invoke(mapInfo);

        _maps[mapId] = mapInfo;

        CurrentMap = mapInfo;


        return mapId;
    }

    public MapInfoObject? GetMapInfo(Guid mapId)
    {
        return _maps.GetValueOrDefault(mapId);
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
