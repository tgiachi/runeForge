using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.MapPrefab;

public class JsonMapPrefabData : BaseJsonEntityData
{
    public int Weight { get; set; }

    public Dictionary<string, JsonMapPrefabFloorData> Floors { get; set; } = new();

    public List<JsonMapPrefabStair> Stairs { get; set; } = new();

}
