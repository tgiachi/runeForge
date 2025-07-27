using Runeforge.Engine.Interfaces.Events;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Events.Delegates;

/// <summary>
///     Disposable subscription for temporary event handlers
/// </summary>
public class EventSubscription<T> : IDisposable where T : IEvent
{
    private readonly IEventBusService _eventBus;
    private readonly IEventHandler<T> _handler;
    private bool _disposed;

    public EventSubscription(IEventBusService eventBus, IEventHandler<T> handler)
    {
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _eventBus.Unsubscribe(_handler);
            _disposed = true;
        }
    }
}
