using Runeforge.Engine.Interfaces.Events;

namespace Runeforge.Engine.Data.Events.Scheduler;

public record AddSchedulerJobEvent(string Name, TimeSpan TotalSpan, Func<Task> Action) : IEvent;
