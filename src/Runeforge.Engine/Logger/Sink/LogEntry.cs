using Serilog.Events;

namespace Runeforge.Engine.Logger.Sink;

/// <summary>
///     Log entry data for delegate sink
/// </summary>
public class LogEntry
{
    public DateTimeOffset Timestamp { get; set; }
    public LogEventLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Exception { get; set; }
    public string Category { get; set; } = string.Empty;
    public string FormattedMessage { get; set; } = string.Empty;
    public LogEvent? OriginalLogEvent { get; set; }
}
