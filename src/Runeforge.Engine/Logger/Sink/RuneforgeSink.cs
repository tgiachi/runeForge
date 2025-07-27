using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;

namespace Runeforge.Engine.Logger.Sink;

/// <summary>
/// Serilog sink that emits log events to a delegate
/// </summary>
public class RuneforgeSink : ILogEventSink
{
    /// <summary>
    /// Delegate type for log events
    /// </summary>
    public delegate void LogEventDelegate(LogEntry logEntry);

    private readonly LogEventDelegate _logDelegate;
    private readonly ITextFormatter _formatter;

    /// <summary>
    /// Create delegate sink with custom formatter
    /// </summary>
    public RuneforgeSink(LogEventDelegate logDelegate, ITextFormatter formatter)
    {
        _logDelegate = logDelegate ?? throw new ArgumentNullException(nameof(logDelegate));
        _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
    }

    /// <summary>
    /// Emit log event to delegate
    /// </summary>
    public void Emit(LogEvent logEvent)
    {
        try
        {
            /// Format the message
            var stringWriter = new StringWriter();
            _formatter.Format(logEvent, stringWriter);
            var formattedMessage = stringWriter.ToString().TrimEnd('\r', '\n');

            /// Extract category from source context if available
            var category = ExtractCategory(logEvent);

            /// Create log entry
            var logEntry = new LogEntry
            {
                Timestamp = logEvent.Timestamp,
                Level = logEvent.Level,
                Message = logEvent.MessageTemplate.Render(logEvent.Properties),
                Exception = logEvent.Exception?.ToString(),
                Category = category,
                FormattedMessage = formattedMessage,
                OriginalLogEvent = logEvent
            };

            /// Emit to delegate
            _logDelegate(logEntry);
        }
        catch (Exception ex)
        {
            /// Avoid infinite recursion if logging fails
            System.Diagnostics.Debug.WriteLine($"DelegateSink error: {ex}");
        }
    }

    /// <summary>
    /// Extract category from log event properties
    /// </summary>
    private static string ExtractCategory(LogEvent logEvent)
    {
        /// Try to get SourceContext (logger name)
        if (logEvent.Properties.TryGetValue("SourceContext", out var sourceContext) &&
            sourceContext is ScalarValue scalarValue &&
            scalarValue.Value is string contextString)
        {
            /// Extract class name from full namespace
            var lastDot = contextString.LastIndexOf('.');
            return lastDot >= 0 ? contextString[(lastDot + 1)..] : contextString;
        }

        /// Try to get Category property
        if (logEvent.Properties.TryGetValue("Category", out var category) &&
            category is ScalarValue categoryScalar &&
            categoryScalar.Value is string categoryString)
        {
            return categoryString;
        }

        /// Default category
        return "General";
    }
}
