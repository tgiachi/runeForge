using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.MapGen;

public class JsonMapGenData : BaseJsonEntityData
{
    public int Width { get; set; }
    public int Height { get; set; }
}
