using Runeforge.Data.Entities.Base;

namespace Runeforge.Data.Entities.Tileset;

public class JsonTilesetData : BaseJsonEntityData
{
    public string FontName { get; set; }

    public int GlyphWidth { get; set; }

    public int GlyphHeight { get; set; }

    public List<JsonTileData> Tiles { get; set; } = new();

    public List<JsonTileAnimationData> Animations { get; set; } = new();

}
