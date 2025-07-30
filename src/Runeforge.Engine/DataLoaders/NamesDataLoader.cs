using Runeforge.Data.Entities.Names;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.DataLoaders;

public class NamesDataLoader : IDataLoader
{
    private readonly INameGeneratorService _nameGeneratorService;

    public NamesDataLoader(INameGeneratorService nameGeneratorService)
    {
        _nameGeneratorService = nameGeneratorService;
    }

    public async Task LoadDataAsync(object data, CancellationToken cancellationToken = default)
    {
        if (data is JsonNameData nameData)
        {
            foreach (var name in nameData.Names)
            {
                _nameGeneratorService.AddName(nameData.Gender, name);
            }
        }
    }
}
