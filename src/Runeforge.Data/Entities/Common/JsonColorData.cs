using System.Text.Json.Serialization;
using Runeforge.Data.Colors;
using Runeforge.Data.Entities.Base;
using Runeforge.Data.Json.Converters;

namespace Runeforge.Data.Entities.Common;

public class JsonColorData : BaseJsonEntityData
{
    public bool IsDefault { get; set; }

    [JsonConverter(typeof(DictionaryStringColorConverter))]
    public Dictionary<string, ColorDef> Colors { get; set; }
}
