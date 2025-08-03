using GoRogue.GameFramework;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Ticks.Actions;
using Runeforge.Engine.Types.Tick;
using SadRogue.Primitives;

namespace Runeforge.Engine.TickActions;

public class MoveAction : BaseAction
{
    public NpcGameObject Npc { get; set; }
    public Direction Direction { get; set; } = Direction.None;

    public MoveAction(NpcGameObject npc, Direction direction)
    {
        Npc = npc;
        Direction = direction;
    }

    public override ActionPriority Priority => ActionPriority.Normal;
    public override int Speed { get; set; } = 1;

    protected override ActionResult ExecuteAction()
    {
        if (Npc.CanMove(Npc.Position + Direction))
        {
            Npc.MoveTo(Direction);
            return ActionResult.Success;
        }

        return ActionResult.Failed;
    }
}
