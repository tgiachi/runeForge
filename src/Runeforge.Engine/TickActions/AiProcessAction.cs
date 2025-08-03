using Runeforge.Engine.Contexts;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Ticks.Actions;
using Runeforge.Engine.Types.Tick;

namespace Runeforge.Engine.TickActions;

public class AiProcessAction : BaseAction
{
    public AiComponent AiComponent { get; set; }

    public Action<AiContext> ProcessAction { get; set; }

    public AiProcessAction(AiComponent aiComponent, Action<AiContext> processAction)
    {
        AiComponent = aiComponent;
        ProcessAction = processAction;
    }

    public override ActionPriority Priority => ActionPriority.Normal;
    public override int Speed { get; set; } = 1;

    protected override ActionResult ExecuteAction()
    {
        ProcessAction(AiComponent.AiContext);

        return ActionResult.Continuing;
    }
}
