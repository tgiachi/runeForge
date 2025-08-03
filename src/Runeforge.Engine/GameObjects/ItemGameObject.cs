using Runeforge.Data.Types.Map;
using SadConsole;
using SadRogue.Integration;
using SadRogue.Primitives;

namespace Runeforge.Engine.GameObjects;

public class ItemGameObject : RogueLikeEntity
{
    public ItemGameObject(
        ColoredGlyph appearance
    ) : base(appearance, false, false, (int)MapLayer.Objects)
    {
        Position = Point.Zero;
    }
}
