using Runeforge.Engine.Data.Internal.Metrics.EventBus;
using Runeforge.Engine.Events;
using Runeforge.Engine.Interfaces.Events;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Services;

/// <summary>
///     Event bus service implementation
/// </summary>
public class EventBusService : IEventBusService
{
    private readonly EventBus _eventBus;

    public EventBusService() => _eventBus = new EventBus();


    public void Subscribe<T>(IEventHandler<T> handler) where T : IEvent
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        _eventBus.Subscribe(handler);
    }

    public void Unsubscribe<T>(IEventHandler<T> handler) where T : IEvent
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        _eventBus.Unsubscribe(handler);
    }

    public void Publish<T>(T eventData) where T : IEvent
    {
        if (eventData == null)
        {
            throw new ArgumentNullException(nameof(eventData));
        }

        _eventBus.Publish(eventData);
    }

    public async Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : IEvent
    {
        if (eventData == null)
        {
            throw new ArgumentNullException(nameof(eventData));
        }

        await Task.Run(() => _eventBus.Publish(eventData), cancellationToken);
    }

    public void CleanupDeadReferences()
    {
        _eventBus.CleanupDeadReferences();
    }

    public EventBusStats GetStats()
    {
        return _eventBus.GetStats();
    }

    public void Clear()
    {
        _eventBus.Clear();
    }

    public bool HasSubscribers<T>() where T : IEvent
    {
        return _eventBus.HasSubscribers<T>();
    }

    public int GetSubscriberCount<T>() where T : IEvent
    {
        return _eventBus.GetSubscriberCount<T>();
    }
}
