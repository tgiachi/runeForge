namespace Runeforge.Engine.Data.Internal.Metrics.EventBus;

/// <summary>
///     Statistics about event bus performance
/// </summary>
public class EventBusStats
{
    public Dictionary<string, (int Alive, int Dead)> HandlerCounts { get; } = new();

    public int TotalAliveHandlers =>
        HandlerCounts.Values.Sum(x => x.Alive);

    public int TotalDeadHandlers =>
        HandlerCounts.Values.Sum(x => x.Dead);
}
