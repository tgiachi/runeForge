using Runeforge.Data.Entities.Base;
using Runeforge.Data.Types.Map;

namespace Runeforge.Data.Entities.MapPrefab;

public class JsonMapPrefabFloorData : BaseJsonEntityData
{
    public string[] Rows { get; set; } = [];

    public string Floor { get; set; }

    public string Wall { get; set; }
    public Dictionary<string, string> Terrain { get; set; } = new();
    public JsonMapPrefabChance[] Items { get; set; }
    public JsonMapPrefabChance[] Npcs { get; set; }
}
