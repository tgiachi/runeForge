using System.Text.Json.Serialization;
using Runeforge.Data.Entities.Base;
using Runeforge.Data.Entities.Tileset;
using Runeforge.Data.Json.Converters;

namespace Runeforge.Data.Entities.Npcs;

public class JsonNpcData : BaseJsonEntityData
{
    public int Level { get; set; } = 1;
    public string Category { get; set; } = string.Empty;
    public string SubCategory { get; set; } = string.Empty;
    public string Gender { get; set; } // This will use for random name
    public JsonHasTile Tile { get; set; }
    public string BrainId { get; set; }
    public JsonNpcInventory Inventory { get; set; } = new();

    [JsonConverter(typeof(RandomValueConverter<int>))]
    public int Gold { get; set; }


}
