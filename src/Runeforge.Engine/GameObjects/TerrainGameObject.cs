using Runeforge.Engine.Types.Map;
using Runeforge.Engine.Utils;
using SadConsole;
using SadRogue.Integration.FieldOfView.Memory;
using SadRogue.Primitives;

namespace Runeforge.Engine.GameObjects;

public class TerrainGameObject : MemoryAwareRogueLikeCell
{
    public ColoredGlyph DarkAppearance { get; }

    public string TileId { get; set; }

    public TerrainGameObject(
        Point position, ColoredGlyph appearance, string tileId,  bool walkable = true, bool transparent = true
    ) : base(position, appearance, (int)MapLayer.Terrain, walkable, transparent)
    {
        TileId = tileId;
        DarkAppearance = ColorUtils.Darken(appearance, 0.5f);
    }
}
