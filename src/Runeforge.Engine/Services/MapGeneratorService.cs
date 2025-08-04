using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class MapGeneratorService : IMapGeneratorService
{
    private readonly ILogger _logger = Log.ForContext<MapGeneratorService>();

    public void AddStep(string name, IMapGenerator generator)
    {
        throw new NotImplementedException();
    }

    public Task ExecuteGenerationAsync(string name)
    {
        throw new NotImplementedException();
    }
}
