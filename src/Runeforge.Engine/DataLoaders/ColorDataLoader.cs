using Newtonsoft.Json.Linq;
using Runeforge.Data.Entities.Common;
using Runeforge.Engine.Extensions.Colors;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.DataLoaders;

public class ColorDataLoader : IDataLoader
{
    private readonly IColorService _colorService;

    public ColorDataLoader(IColorService colorService)
    {
        _colorService = colorService;
    }

    public async Task LoadDataAsync(object data, CancellationToken cancellationToken = default)
    {
        if (data is JsonColorData colorData)
        {
            foreach (var color in colorData.Colors)
            {
                _colorService.AddColor(colorData.Id, color.Key, color.Value.ToColor());
            }

            if (colorData.IsDefault)
            {
                _colorService.SetDefaultColorSet(colorData.Id);
            }
        }
    }
}
