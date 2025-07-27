using Runeforge.Engine.Interfaces.Events;

namespace Runeforge.Engine.Events.Delegates;

/// <summary>
///     Wrapper for async delegate-based event handlers
/// </summary>
internal class AsyncDelegateEventHandler<T> : IEventHandler<T> where T : IEvent
{
    private readonly Func<T, Task> _handler;

    public AsyncDelegateEventHandler(Func<T, Task> handler) =>
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));

    public void Handle(T eventData)
    {
        /// Fire and forget for async handlers in sync context
        _ = Task.Run(() => _handler(eventData));
    }
}
