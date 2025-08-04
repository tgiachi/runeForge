using Runeforge.Data.Entities.MapGen;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.DataLoaders;

public class MapGenDataLoader : IDataLoader
{
    private readonly IMapGeneratorService _mapGeneratorService;

    public MapGenDataLoader(IMapGeneratorService mapGeneratorService)
    {
        _mapGeneratorService = mapGeneratorService;
    }

    public async Task LoadDataAsync(object data, CancellationToken cancellationToken = default)
    {
        if (data is JsonMapGenData mapGenData)
        {
            _mapGeneratorService.AddMapGenerator(mapGenData);
        }
    }
}
