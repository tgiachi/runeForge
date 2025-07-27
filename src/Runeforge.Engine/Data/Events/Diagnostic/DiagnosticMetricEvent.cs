using Runeforge.Engine.Data.Metrics.Diagnostic;
using Runeforge.Engine.Interfaces.Events;

namespace Runeforge.Engine.Data.Events.Diagnostic;

public record DiagnosticMetricEvent(MetricProviderData Metrics) : IEvent;
