namespace Runeforge.Engine.Interfaces.Services;

public interface ITickSystemService
{
    delegate void TickDelegate(int tickCount);


    event TickDelegate Tick;
    event TickDelegate TickStarted;
    event TickDelegate TickEnded;

}
