namespace Runeforge.Engine.Interfaces.Events;

/// <summary>
///     Event handler interface
/// </summary>
public interface IEventHandler
{
}

/// <summary>
///     Generic event handler interface
/// </summary>
public interface IEventHandler<in T> : IEventHandler where T : IEvent
{
    void Handle(T eventData);
}
