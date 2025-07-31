using GoRogue.GameFramework;
using Runeforge.Engine.Data;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.Interfaces.Services.Base;
using SadRogue.Integration.Maps;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapService : IRuneforgeStartableService
{
    delegate Task MapGeneratedHandler(MapInfoObject mapInfo);

    delegate void MapStartGeneratedHandler(Guid id);


    event MapGeneratedHandler? MapGenerated;

    event MapStartGeneratedHandler? MapStartGenerated;

    DefaultRendererParams DefaultRendererParams { get; set; }
    MapInfoObject CurrentMap { get; set; }
    Task<Guid> GenerateMapAsync(int width, int height, string name, string description, int level = 1, CancellationToken cancellationToken = default);
    MapInfoObject? GetMapInfo(Guid mapId);

}
