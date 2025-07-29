using Runeforge.Engine.Interfaces.Ticks;
using Runeforge.Engine.Ticks;

namespace Runeforge.Engine.Extensions.Ticks;

/// <summary>
/// Extension methods for ActionQueue
/// </summary>
public static class ActionQueueExtensions
{
    /// <summary>
    /// Add action only if it's not already in the queue
    /// </summary>
    public static bool TryEnqueue(this ActionQueue queue, ITickAction action)
    {
        if (queue.Contains(action))
            return false;

        queue.Enqueue(action);
        return true;
    }

    /// <summary>
    /// Get execution statistics from results
    /// </summary>
    public static ActionQueueStats GetStats(this List<ActionExecutionSummary> results)
    {
        return new ActionQueueStats(
            TotalActions: results.Count,
            SuccessfulActions: results.Count(r => r.WasSuccessful),
            FailedActions: results.Count(r => !r.WasSuccessful),
            TotalDuration: results.Sum(r => r.Duration.TotalMilliseconds),
            AverageDuration: results.Count > 0 ? results.Average(r => r.Duration.TotalMilliseconds) : 0
        );
    }
}
