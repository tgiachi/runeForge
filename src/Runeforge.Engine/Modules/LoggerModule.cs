using Runeforge.Engine.Attributes.Scripts;
using Serilog;

namespace Runeforge.Engine.Modules;

[ScriptModule("logger")]
public class LoggerModule
{
    private readonly ILogger _logger = Log.ForContext<LoggerModule>();

    [ScriptFunction("Log message with info level")]
    public void Info(string message, params object[] args)
    {
        if (args.Length > 0)
        {
            message = string.Format(message, args);
        }

        _logger.Information(message);
    }

    [ScriptFunction("Log message with warning level")]
    public void Warning(string message, params object[] args)
    {
        if (args.Length > 0)
        {
            message = string.Format(message, args);
        }

        _logger.Warning(message);
    }

    [ScriptFunction("Log message with error level")]
    public void Error(string message, params object[] args)
    {
        if (args.Length > 0)
        {
            message = string.Format(message, args);
        }

        _logger.Error(message);
    }

    [ScriptFunction("Log message with debug level")]
    public void Debug(string message, params object[] args)
    {
        if (args.Length > 0)
        {
            message = string.Format(message, args);
        }

        _logger.Debug(message);
    }
}
