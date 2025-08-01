using Runeforge.Data.Entities.Items;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.DataLoaders;

public class ItemDataLoader : IDataLoader
{
    private readonly IItemService _itemService;

    public ItemDataLoader(IItemService itemService)
    {
        _itemService = itemService;
    }

    public async Task LoadDataAsync(object data, CancellationToken cancellationToken = default)
    {
        if (data is JsonItemData itemData)
        {
            _itemService.AddItem(itemData);
        }
    }
}
