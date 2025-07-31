using Runeforge.Engine.GameObjects.Components;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Engine.GameObjects;

public class PlayerGameObject : NpcGameObject
{
    public PlayerGameObject(Point position, ColoredGlyph appearance) : base(position, appearance)
    {
    }

    public void ShowAllMap()
    {
        CurrentMap.PlayerFOV.Calculate(Position, 800);
        GoRogueComponents.GetFirstOrDefault<PlayerFOVController>().FOVRadius = 800;
        GoRogueComponents.GetFirstOrDefault<PlayerFOVController>().CalculateFOV();
    }

    public void UpdateFOV()
    {
        GoRogueComponents.GetFirstOrDefault<PlayerFOVController>()?.CalculateFOV();
    }
}
