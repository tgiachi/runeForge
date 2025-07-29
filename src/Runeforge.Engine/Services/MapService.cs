using GoRogue.GameFramework;
using GoRogue.MapGeneration;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Types.Map;
using SadRogue.Primitives;

namespace Runeforge.Engine.Services;

public class MapService : IMapService
{
    public Map CurrentMap { get; set; }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        var generator = new Generator(300, 300)
            .ConfigAndGenerateSafe(c => c.AddComponent(DefaultAlgorithms.RectangleMapSteps()))
            .Generate();

        CurrentMap = new Map(300, 300, Enum.GetValues<MapLayer>().Length, Distance.Chebyshev);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
    }
}
