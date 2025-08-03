using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Tileset;

public class JsonTilesetData : BaseJsonEntityData
{
    public List<JsonTileData> Tiles { get; set; } = new();

    public List<JsonTileAnimationData> Animations { get; set; } = new();
}
