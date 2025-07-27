using Runeforge.Engine.Interfaces.Events;
using Runeforge.Engine.Interfaces.Metrics;

namespace Runeforge.Engine.Data.Events.Diagnostic;

public record RegisterMetricEvent(IMetricsProvider provider) : IEvent;
