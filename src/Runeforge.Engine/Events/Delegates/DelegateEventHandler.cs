using Runeforge.Engine.Interfaces.Events;

namespace Runeforge.Engine.Events.Delegates;

/// <summary>
///     Wrapper for delegate-based event handlers
/// </summary>
internal class DelegateEventHandler<T> : IEventHandler<T> where T : IEvent
{
    private readonly Action<T> _handler;

    public DelegateEventHandler(Action<T> handler) => _handler = handler ?? throw new ArgumentNullException(nameof(handler));

    public void Handle(T eventData)
    {
        _handler(eventData);
    }
}
