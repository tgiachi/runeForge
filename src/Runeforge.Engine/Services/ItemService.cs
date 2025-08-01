using System.Text.RegularExpressions;
using Runeforge.Core.Extensions.Rnd;
using Runeforge.Core.Utils;
using Runeforge.Data.Entities.Items;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public partial class ItemService : IItemService
{
    [GeneratedRegex(@"^cat:(.+)\|(.+)$")]
    private static partial Regex CategorySearchRegex();

    private readonly ILogger _logger = Log.ForContext<ItemService>();

    private readonly Dictionary<string, JsonItemData> _idItemDataMap = new();
    private readonly Dictionary<string, List<JsonItemData>> _itemDataCategory = new();
    private readonly Dictionary<string, List<JsonItemData>> _itemDataTag = new();

    private readonly ITileSetService _tileSetService;

    public ItemService(ITileSetService tileSetService)
    {
        _tileSetService = tileSetService;
    }

    public void AddItem(JsonItemData item)
    {
        _idItemDataMap.Add(item.Id, item);

        if (string.IsNullOrEmpty(item.SubCategory))
        {
            item.SubCategory = "none";
        }

        if (string.IsNullOrEmpty(item.Category))
        {
            item.Category = "none";
        }

        var hashedCategory = HashUtils.ComputeSha256Hash($"{item.Category.ToLower()}:{item.SubCategory.ToLower()}");

        if (!_itemDataCategory.TryGetValue(hashedCategory, out var itemList))
        {
            itemList = new List<JsonItemData>();
            _itemDataCategory[hashedCategory] = itemList;
        }

        if (item.Tags != null && item.Tags.Count > 0)
        {
            foreach (var tag in item.Tags)
            {
                if (string.IsNullOrEmpty(tag)) continue;

                var normalizedTag = tag.ToLower();
                if (!_itemDataTag.TryGetValue(normalizedTag, out var tagList))
                {
                    tagList = new List<JsonItemData>();
                    _itemDataTag[normalizedTag] = tagList;
                }

                tagList.Add(item);
            }
        }

        _logger.Debug(
            "Added item {ItemId} to category {Category}/{SubCategory}",
            item.Id,
            item.Category,
            item.SubCategory
        );

        itemList.Add(item);
    }

    public ItemGameObject CreateItemGameObject(string searchCriteria)
    {
        if (string.IsNullOrWhiteSpace(searchCriteria))
            return null;

        JsonItemData? itemData = null;

        // 1. Try by ID first
        if (_idItemDataMap.TryGetValue(searchCriteria, out itemData))
        {
            return CreateItemGameObjectFromData(itemData);
        }

        // 2. Check if it's a category search pattern "cat:category|subcategory"
        var categoryMatch = CategorySearchRegex().Match(searchCriteria);
        if (categoryMatch.Success)
        {
            var category = categoryMatch.Groups[1].Value.ToLower();
            var subCategory = categoryMatch.Groups[2].Value.ToLower();
            var hashedCategory = HashUtils.ComputeSha256Hash($"{category}:{subCategory}");

            if (_itemDataCategory.TryGetValue(hashedCategory, out var categoryItems) && categoryItems.Count > 0)
            {
                itemData = categoryItems.RandomItem();
                if (itemData != null)
                {
                    return CreateItemGameObjectFromData(itemData);
                }
            }
        }

        // 3. Try by tag
        var normalizedTag = searchCriteria.ToLower();
        if (_itemDataTag.TryGetValue(normalizedTag, out var tagItems) && tagItems.Count > 0)
        {
            itemData = tagItems.RandomItem();
            if (itemData != null)
            {
                return CreateItemGameObjectFromData(itemData);
            }
        }

        return null;
    }

    /// <summary>
    /// Create item game object from item data
    /// </summary>
    /// <param name="itemData">Item data</param>
    /// <returns>Item game object</returns>
    private ItemGameObject CreateItemGameObjectFromData(JsonItemData itemData)
    {
        var tile = _tileSetService.CreateGlyph(itemData.Tile);

        var itemGameObject = new ItemGameObject(tile.ColoredGlyph);

        return itemGameObject;
    }
}
