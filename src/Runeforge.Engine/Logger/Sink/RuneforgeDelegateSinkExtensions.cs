using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace Runeforge.Engine.Logger.Sink;

/// <summary>
///     Serilog configuration extensions for delegate sink
/// </summary>
public static class RuneforgeDelegateSinkExtensions
{
    /// <summary>
    ///     Add delegate sink to Serilog configuration
    /// </summary>
    public static LoggerConfiguration Delegate(
        this LoggerSinkConfiguration sinkConfiguration,
        RuneforgeSink.LogEventDelegate logDelegate,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose,
        string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
    )
    {
        ArgumentNullException.ThrowIfNull(sinkConfiguration);

        ArgumentNullException.ThrowIfNull(logDelegate);

        var formatter = new MessageTemplateTextFormatter(outputTemplate);
        var sink = new RuneforgeSink(logDelegate, formatter);

        return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
    }

    /// <summary>
    ///     Add delegate sink with custom formatter
    /// </summary>
    public static LoggerConfiguration Delegate(
        this LoggerSinkConfiguration sinkConfiguration,
        RuneforgeSink.LogEventDelegate logDelegate,
        ITextFormatter formatter,
        LogEventLevel restrictedToMinimumLevel = LogEventLevel.Verbose
    )
    {
        ArgumentNullException.ThrowIfNull(sinkConfiguration);

        ArgumentNullException.ThrowIfNull(logDelegate);

        ArgumentNullException.ThrowIfNull(formatter);

        var sink = new RuneforgeSink(logDelegate, formatter);
        return sinkConfiguration.Sink(sink, restrictedToMinimumLevel);
    }
}
