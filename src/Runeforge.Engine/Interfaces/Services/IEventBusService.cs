using Runeforge.Engine.Data.Internal.Metrics.EventBus;
using Runeforge.Engine.Interfaces.Events;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

/// <summary>
///     Service interface for event bus operations
/// </summary>
public interface IEventBusService : IRuneforgeService
{
    /// <summary>
    ///     Subscribe to events of type T
    /// </summary>
    void Subscribe<T>(IEventHandler<T> handler) where T : IEvent;

    /// <summary>
    ///     Unsubscribe from events of type T
    /// </summary>
    void Unsubscribe<T>(IEventHandler<T> handler) where T : IEvent;

    /// <summary>
    ///     Publish event immediately (synchronous)
    /// </summary>
    void Publish<T>(T eventData) where T : IEvent;

    /// <summary>
    ///     Publish event asynchronously
    /// </summary>
    Task PublishAsync<T>(T eventData, CancellationToken cancellationToken = default) where T : IEvent;

    /// <summary>
    ///     Clean up dead weak references
    /// </summary>
    void CleanupDeadReferences();

    /// <summary>
    ///     Get event bus statistics
    /// </summary>
    EventBusStats GetStats();

    /// <summary>
    ///     Clear all handlers (useful for testing)
    /// </summary>
    void Clear();

    /// <summary>
    ///     Check if there are any subscribers for event type T
    /// </summary>
    bool HasSubscribers<T>() where T : IEvent;

    /// <summary>
    ///     Get count of active subscribers for event type T
    /// </summary>
    int GetSubscriberCount<T>() where T : IEvent;

    /// <summary>
    ///   Observable that emits all events
    /// </summary>
    IObservable<object> AllEventsObservable { get; }
}
