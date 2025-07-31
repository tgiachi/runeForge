using Runeforge.Engine.Types.Map;
using SadConsole;
using SadRogue.Integration;
using SadRogue.Primitives;

namespace Runeforge.Engine.GameObjects;

public class NpcGameObject : RogueLikeEntity
{
    public string Name { get; set; }

    public bool IsDead { get; set; }

    public event EventHandler<object> Die;

    public void OnDie()
    {
        Die?.Invoke(this, null);
    }


    public NpcGameObject(
        Point position, ColoredGlyph appearance
    ) : base(appearance, false, false, (int)MapLayer.Entities)
    {
        Position = position;
    }

    public void MoveTo(Direction direction)
    {
        Position += direction;
    }

    public override string ToString() => $"ID: {ID} Npc: {Name}";
}
