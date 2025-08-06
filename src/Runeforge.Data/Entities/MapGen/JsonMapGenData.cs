using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.MapGen;

public class JsonMapGenData : BaseJsonEntityData
{
    public bool IsDefault { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public List<JsonMapGenStep> Steps { get; set; } = new();
}
