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

}
