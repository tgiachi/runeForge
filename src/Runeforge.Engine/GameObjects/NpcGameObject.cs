using GoRogue.GameFramework;
using Runeforge.Data.Types.Map;
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
        var newPosition = Position + direction;
        if (this.CanMove(newPosition))
        {
            Position += direction;
        }
    }

    public void MoveTo(Point newPosition)
    {
        if (this.CanMove(newPosition))
        {
            Position = newPosition;
        }
    }

    public override string ToString() => $"ID: {ID} Npc: {Name}";
}
