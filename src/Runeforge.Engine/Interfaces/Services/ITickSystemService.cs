using Runeforge.Engine.Interfaces.Services.Base;
using Runeforge.Engine.Interfaces.Ticks;

namespace Runeforge.Engine.Interfaces.Services;

public interface ITickSystemService : IRuneforgeService
{
    delegate void TickDelegate(int tickCount);

    int TickCount { get; }

    event TickDelegate Tick;
    event TickDelegate TickStarted;
    event TickDelegate TickEnded;

    void ExecuteTick();

    void EnqueueActions(IEnumerable<ITickAction> actions);

    void EnqueueAction(ITickAction action);

    void ClearContinuingActions();
}
