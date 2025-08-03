using Runeforge.Data.Entities.Base;
using Runeforge.Data.Entities.Tileset;
using Runeforge.Data.Types.Items;

namespace Runeforge.Data.Entities.Items;

public class JsonItemData : BaseJsonEntityData
{
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public JsonHasTile Tile { get; set; }
    public JsonItemStatData Stats { get; set; }
    public JsonItemContainerData? Container { get; set; }
    public JsonLightSourceData? LightSource { get; set; }
    public ItemFlagType[] Flags { get; set; }

    public bool IsContainer => Container != null;

    public bool IsLightSource => LightSource != null;

    public bool HasFlag(ItemFlagType flag)
    {
        return Flags?.Contains(flag) ?? false;
    }
}
