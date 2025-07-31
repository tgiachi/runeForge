using Runeforge.Data.Entities.Tileset;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.DataLoaders;

public class TileSetDataLoader : IDataLoader
{
    private readonly ITileSetService _tileSetService;

    public TileSetDataLoader(ITileSetService tileSetService)
    {
        _tileSetService = tileSetService;
    }

    public async Task LoadDataAsync(object data, CancellationToken cancellationToken = default)
    {
        if (data is JsonTilesetData tileSetData)
        {
            foreach(var tile in tileSetData.Tiles)
            {
                _tileSetService.AddTile(tileSetData.Id, tile);
            }
        }
    }
}
