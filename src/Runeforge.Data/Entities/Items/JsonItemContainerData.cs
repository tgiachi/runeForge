using System.Text.Json.Serialization;
using Runeforge.Data.Json.Converters;

namespace Runeforge.Data.Entities.Items;

public class JsonItemContainerData
{
    public Dictionary<string, string> Items { get; set; } = new();

    [JsonConverter(typeof(RandomValueConverter<int>))]
    public int Capacity { get; set; }


}
