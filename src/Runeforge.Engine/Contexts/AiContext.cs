using Runeforge.Core.Extensions.Rnd;
using Runeforge.Engine.GameObjects;
using SadRogue.Primitives;

namespace Runeforge.Engine.Contexts;

public class AiContext
{
    public static AiContext Create(NpcGameObject self, PlayerGameObject player)
    {
        return new AiContext
        {
            Self = self,
            Player = player
        };
    }

    public PlayerGameObject Player { get; set; }
    public NpcGameObject Self { get; set; }

    public void Say(string text)
    {

    }

    public void MoveUp()
    {
        Self.MoveTo(Direction.Up);
    }

    public void MoveDown()
    {
        Self.MoveTo(Direction.Down);
    }

    public void MoveLeft()
    {
        Self.MoveTo(Direction.Left);
    }

    public void MoveRight()
    {
        Self.MoveTo(Direction.Right);
    }

    public void MoveRandomly()
    {
        var direction = new Point(
            Random.Shared.Next(0, 2) == 0 ? -1 : 1,
            Random.Shared.Next(0, 2) == 0 ? -1 : 1
        );

        var newPosition = Self.Position + direction;

        Self.MoveTo(newPosition);
    }

}
