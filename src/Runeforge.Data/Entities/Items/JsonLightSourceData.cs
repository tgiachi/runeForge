using System.Text.Json.Serialization;
using Runeforge.Data.Json.Converters;

namespace Runeforge.Data.Entities.Items;

public class JsonLightSourceData
{
    [JsonConverter(typeof(RandomValueConverter<int>))]
    public int Radius { get; set; }

    public override string ToString()
    {
        return "Radius: " + Radius;
    }
}
