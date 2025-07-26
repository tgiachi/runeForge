using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Tileset;

public class JsonTileAnimationData : BaseJsonEntityData
{
    public List<string> Frames { get; set; } = new();

    public int Duration { get; set; } = 400;

    public bool Loop { get; set; } = true;

}
