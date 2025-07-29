using Runeforge.Engine.Types.Tick;

namespace Runeforge.Engine.Ticks.Actions;

public class ScriptedAction : BaseAction
{
    private Action _action;

    public ScriptedAction(Action action)
    {
        _action = action;
    }

    public override ActionPriority Priority { get; } = ActionPriority.Normal;
    public override int Speed { get; set; } = 1;
    protected override ActionResult ExecuteAction()
    {
        return ActionResult.Success;
    }
}
