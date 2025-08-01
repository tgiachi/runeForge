using System.Text.RegularExpressions;
using Runeforge.Core.Extensions.Rnd;
using Runeforge.Core.Utils;
using Runeforge.Data.Entities.Npcs;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Primitives;
using Serilog;

namespace Runeforge.Engine.Services;

public partial class NpcService : INpcService
{
    [GeneratedRegex(@"^cat:(.+)\|(.+)$")]
    private static partial Regex CategorySearchRegex();

    private readonly ILogger _logger = Log.ForContext<NpcService>();

    private readonly IItemService _itemService;
    private readonly ITileSetService _tileSetService;

    private readonly Dictionary<string, JsonNpcData> _npcIdDataMap = new();
    private readonly Dictionary<string, List<JsonNpcData>> _npcCategoryMap = new();
    private readonly Dictionary<string, List<JsonNpcData>> _npcTagsMap = new();

    public NpcService(IItemService itemService, ITileSetService tileSetService)
    {
        _itemService = itemService;
        _tileSetService = tileSetService;
    }

    public void AddNpc(JsonNpcData npc)
    {
        _npcIdDataMap.Add(npc.Id, npc);

        if (string.IsNullOrEmpty(npc.SubCategory))
        {
            npc.SubCategory = "none";
        }

        if (string.IsNullOrEmpty(npc.Category))
        {
            npc.Category = "none";
        }

        var hashedCategory = HashUtils.ComputeSha256Hash($"{npc.Category.ToLower()}:{npc.SubCategory.ToLower()}");

        if (!_npcCategoryMap.TryGetValue(hashedCategory, out var npcList))
        {
            npcList = new List<JsonNpcData>();
            _npcCategoryMap[hashedCategory] = npcList;
        }

        if (npc.Tags != null && npc.Tags.Count > 0)
        {
            foreach (var tag in npc.Tags)
            {
                if (string.IsNullOrEmpty(tag)) continue;

                var normalizedTag = tag.ToLower();
                if (!_npcTagsMap.TryGetValue(normalizedTag, out var tagList))
                {
                    tagList = new List<JsonNpcData>();
                    _npcTagsMap[normalizedTag] = tagList;
                }

                tagList.Add(npc);
            }
        }

        _logger.Debug(
            "Added item {ItemId} to category {Category}/{SubCategory}",
            npc.Id,
            npc.Category,
            npc.SubCategory
        );

        npcList.Add(npc);
    }

    public NpcGameObject CreateNpcGameObject(string searchCriteria)
    {
        if (string.IsNullOrWhiteSpace(searchCriteria))
        {
            return null;
        }

        JsonNpcData? npcData = null;

        // 1. Try by ID first
        if (_npcIdDataMap.TryGetValue(searchCriteria, out npcData))
        {
            return CreateNpcGameObjectFromData(npcData);
        }

        // 2. Check if it's a category search pattern "cat:category|subcategory"
        var categoryMatch = CategorySearchRegex().Match(searchCriteria);
        if (categoryMatch.Success)
        {
            var category = categoryMatch.Groups[1].Value.ToLower();
            var subCategory = categoryMatch.Groups[2].Value.ToLower();
            var hashedCategory = HashUtils.ComputeSha256Hash($"{category}:{subCategory}");

            if (_npcCategoryMap.TryGetValue(hashedCategory, out var categoryItems) && categoryItems.Count > 0)
            {
                npcData = categoryItems.RandomItem();
                if (npcData != null)
                {
                    return CreateNpcGameObjectFromData(npcData);
                }
            }
        }

        // 3. Try by tag
        var normalizedTag = searchCriteria.ToLower();
        if (_npcTagsMap.TryGetValue(normalizedTag, out var tagItems) && tagItems.Count > 0)
        {
            npcData = tagItems.RandomItem();
            if (npcData != null)
            {
                return CreateNpcGameObjectFromData(npcData);
            }
        }

        return null;
    }

    private NpcGameObject CreateNpcGameObjectFromData(JsonNpcData npcData)
    {
        var tile = _tileSetService.CreateGlyph(npcData.Tile);
        var npcGameObject = new NpcGameObject(Point.Zero, tile.ColoredGlyph);

        return npcGameObject;
    }
}
