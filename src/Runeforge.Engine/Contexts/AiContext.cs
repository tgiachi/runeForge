using Runeforge.Engine.GameObjects;
using SadRogue.Primitives;

namespace Runeforge.Engine.Contexts;

public class AiContext
{
    public Point PlayerPosition { get; set; }
    public NpcGameObject Self { get; set; }

    public void Say(string text)
    {

    }

}
