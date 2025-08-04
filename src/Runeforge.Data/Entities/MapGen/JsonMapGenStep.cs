using System.Text.Json.Serialization;

namespace Runeforge.Data.Entities.MapGen;

public class JsonMapGenStep
{
    public string StepName { get; set; }

    [JsonPropertyName("props")]
    public Dictionary<string, object> Properties { get; set; } = new();

}
