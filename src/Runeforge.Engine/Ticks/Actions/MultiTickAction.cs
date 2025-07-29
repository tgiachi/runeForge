using Runeforge.Engine.Interfaces.Ticks;
using Runeforge.Engine.Types.Tick;

namespace Runeforge.Engine.Ticks.Actions;

public abstract class MultiTickAction : ITickAction
{
    public Guid Id { get; } = Guid.NewGuid();
    public abstract ActionPriority Priority { get; }
    public abstract int Speed { get; set; }

    public int RemainingTicks { get; private set; }
    public int TotalTicks { get; }

    protected MultiTickAction(int totalTicks)
    {
        TotalTicks = totalTicks;
        RemainingTicks = totalTicks;
    }

    public virtual bool CanBeExecuted() => true;

    public ActionResult Execute()
    {
        var tickResult = ExecuteTick();

        if (tickResult != ActionResult.Success)
        {
            return tickResult;
        }

        RemainingTicks--;

        if (RemainingTicks > 0)
        {
            return ActionResult.Continuing;
        }

        // Finito!
        return ActionResult.Success;
    }


    protected abstract ActionResult ExecuteTick();

    /// <summary>
    /// Get progress as percentage (0.0 to 1.0)
    /// </summary>
    public double Progress => 1.0 - (double)RemainingTicks / TotalTicks;

    /// <summary>
    /// Check if this is the first tick of the action
    /// </summary>
    public bool IsFirstTick => RemainingTicks == TotalTicks;

    /// <summary>
    /// Check if this is the last tick of the action
    /// </summary>
    public bool IsLastTick => RemainingTicks == 1;
}
