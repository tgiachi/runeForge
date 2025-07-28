using Runeforge.Engine.Logger.Sink;
using Runeforge.Engine.Types;

namespace Runeforge.Engine.Data.Options;

public class RuneforgeOptions
{
    public string RootDirectory { get; set; }

    public LogLevelType LogLevel { get; set; } = LogLevelType.Information;

    public bool LogToFile { get; set; } = true;

    public bool LogToConsole { get; set; } = true;

    public string GameName { get; set; }



}
