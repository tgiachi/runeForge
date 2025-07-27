using Runeforge.Engine.Events.Delegates;
using Runeforge.Engine.Interfaces.Events;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Extensions.EventBus;

/// <summary>
///     Extensions for IEventBusService to support functional handlers
/// </summary>
public static class EventBusServiceExtensions
{
    /// <summary>
    ///     Subscribe with a lambda/delegate instead of implementing interface
    /// </summary>
    public static void Subscribe<T>(this IEventBusService eventBus, Action<T> handler) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        var wrapper = new DelegateEventHandler<T>(handler);
        eventBus.Subscribe(wrapper);
    }

    /// <summary>
    ///     Subscribe with async lambda/delegate
    /// </summary>
    public static void Subscribe<T>(this IEventBusService eventBus, Func<T, Task> handler) where T : IEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        var wrapper = new AsyncDelegateEventHandler<T>(handler);
        eventBus.Subscribe(wrapper);
    }

    /// <summary>
    ///     Subscribe temporarily with automatic unsubscribe via IDisposable
    /// </summary>
    public static IDisposable SubscribeTemporary<T>(this IEventBusService eventBus, IEventHandler<T> handler)
        where T : IEvent
    {
        eventBus.Subscribe(handler);
        return new EventSubscription<T>(eventBus, handler);
    }

    /// <summary>
    ///     Subscribe temporarily with lambda
    /// </summary>
    public static IDisposable SubscribeTemporary<T>(this IEventBusService eventBus, Action<T> handler) where T : IEvent
    {
        var wrapper = new DelegateEventHandler<T>(handler);
        eventBus.Subscribe(wrapper);
        return new EventSubscription<T>(eventBus, wrapper);
    }
}
