using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Runeforge.Engine.Data.Internal.Metrics.EventBus;
using Runeforge.Engine.Interfaces.Events;

namespace Runeforge.Engine.Events;

/// <summary>
///     High-performance event bus using type-based routing and weak references
/// </summary>
public class EventBus
{
    /// <summary>
    ///     Thread-safe storage for event handlers by type
    /// </summary>
    private readonly ConcurrentDictionary<Type, ConcurrentBag<WeakReference<IEventHandler>>> _handlers = new();

    /// <summary>
    ///     Object pool for reusing delegate allocations
    /// </summary>
    private readonly ConcurrentBag<List<IEventHandler>> _listPool = new();

    /// <summary>
    ///     Subscribe to events of type T
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Subscribe<T>(IEventHandler<T> handler) where T : IEvent
    {
        var eventType = typeof(T);
        var weakRef = new WeakReference<IEventHandler>(handler);

        _handlers.AddOrUpdate(
            eventType,
            new ConcurrentBag<WeakReference<IEventHandler>> { weakRef },
            (key, existing) =>
            {
                existing.Add(weakRef);
                return existing;
            }
        );
    }

    /// <summary>
    ///     Unsubscribe from events of type T
    /// </summary>
    public void Unsubscribe<T>(IEventHandler<T> handler) where T : IEvent
    {
        var eventType = typeof(T);

        if (!_handlers.TryGetValue(eventType, out var handlers))
        {
            return;
        }

        /// Create new bag without the handler (ConcurrentBag doesn't support removal)
        var newHandlers = new ConcurrentBag<WeakReference<IEventHandler>>();

        foreach (var weakRef in handlers)
        {
            if (weakRef.TryGetTarget(out var target) && !ReferenceEquals(target, handler))
            {
                newHandlers.Add(weakRef);
            }
        }

        _handlers.TryUpdate(eventType, newHandlers, handlers);
    }

    /// <summary>
    ///     Publish event immediately (synchronous)
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Publish<T>(T eventData) where T : IEvent
    {
        var eventType = typeof(T);

        if (!_handlers.TryGetValue(eventType, out var weakHandlers))
        {
            return;
        }

        /// Get pooled list or create new one
        var activeHandlers = GetPooledList();

        try
        {
            /// Collect alive handlers
            foreach (var weakRef in weakHandlers)
            {
                if (weakRef.TryGetTarget(out var handler))
                {
                    activeHandlers.Add(handler);
                }
            }

            /// Execute handlers
            foreach (var handler in activeHandlers)
            {
                if (handler is IEventHandler<T> typedHandler)
                {
                    typedHandler.Handle(eventData);
                }
            }
        }
        finally
        {
            /// Return to pool
            ReturnToPool(activeHandlers);
        }
    }

    /// <summary>
    ///     Clean up dead weak references periodically
    /// </summary>
    public void CleanupDeadReferences()
    {
        foreach (var kvp in _handlers)
        {
            var eventType = kvp.Key;
            var handlers = kvp.Value;

            var aliveHandlers = new ConcurrentBag<WeakReference<IEventHandler>>();

            foreach (var weakRef in handlers)
            {
                if (weakRef.TryGetTarget(out _))
                {
                    aliveHandlers.Add(weakRef);
                }
            }

            _handlers.TryUpdate(eventType, aliveHandlers, handlers);
        }
    }

    /// <summary>
    ///     Get statistics about event bus
    /// </summary>
    public EventBusStats GetStats()
    {
        var stats = new EventBusStats();

        foreach (var kvp in _handlers)
        {
            var aliveCount = 0;
            var deadCount = 0;

            foreach (var weakRef in kvp.Value)
            {
                if (weakRef.TryGetTarget(out _))
                {
                    aliveCount++;
                }
                else
                {
                    deadCount++;
                }
            }

            stats.HandlerCounts[kvp.Key.Name] = (aliveCount, deadCount);
        }

        return stats;
    }

    /// <summary>
    ///     Clear all handlers (useful for testing)
    /// </summary>
    public void Clear()
    {
        _handlers.Clear();
    }

    /// <summary>
    ///     Check if there are any subscribers for event type T
    /// </summary>
    public bool HasSubscribers<T>() where T : IEvent
    {
        var eventType = typeof(T);

        if (!_handlers.TryGetValue(eventType, out var handlers))
        {
            return false;
        }

        foreach (var weakRef in handlers)
        {
            if (weakRef.TryGetTarget(out _))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///     Get count of active subscribers for event type T
    /// </summary>
    public int GetSubscriberCount<T>() where T : IEvent
    {
        var eventType = typeof(T);

        if (!_handlers.TryGetValue(eventType, out var handlers))
        {
            return 0;
        }

        var count = 0;
        foreach (var weakRef in handlers)
        {
            if (weakRef.TryGetTarget(out _))
            {
                count++;
            }
        }

        return count;
    }

    /// <summary>
    ///     Object pooling for performance
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private List<IEventHandler> GetPooledList()
    {
        if (_listPool.TryTake(out var list))
        {
            list.Clear();
            return list;
        }

        return new List<IEventHandler>(16); /// Pre-allocate reasonable size
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReturnToPool(List<IEventHandler> list)
    {
        if (list.Capacity <= 64) /// Don't pool huge lists
        {
            _listPool.Add(list);
        }
    }
}
