using System.Text.Json.Serialization;
using Runeforge.Data.Json.Converters;

namespace Runeforge.Data.Entities.Items;

public class JsonItemStatData
{
    [JsonConverter(typeof(RandomValueConverter<double>))]
    public double Weight { get; set; } = 1;


    [JsonConverter(typeof(RandomValueConverter<int>))]
    public int Value { get; set; } = 0;


    public override string ToString()
    {
        return "Weight: " + Weight + ", Value: " + Value;
    }
}
