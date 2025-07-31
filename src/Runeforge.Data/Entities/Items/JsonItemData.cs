using System.Text.Json.Serialization;
using Runeforge.Data.Entities.Base;
using Runeforge.Data.Entities.Tileset;
using Runeforge.Data.Json.Converters;
using Runeforge.Data.Types.Items;

namespace Runeforge.Data.Entities.Items;

public class JsonItemData : BaseJsonEntityData
{
    public string Category { get; set; }
    public string SubCategory { get; set; }

    public JsonHasTile Tile { get; set; }

    [JsonConverter(typeof(RandomValueConverter<int>))]
    public int Weight { get; set; } = 1;


    [JsonConverter(typeof(RandomValueConverter<int>))]
    public int Value { get; set; } = 0;

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
