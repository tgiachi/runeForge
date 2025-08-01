using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class AiService : IAiService
{
    private readonly Dictionary<string, Action<AiContext>> _brains = new();

    private readonly ILogger _logger = Log.ForContext<AiService>();

    private readonly Dictionary<string, AiContext> _contexts = new();

    public void AddBrain(string name, Action<AiContext> action)
    {
        _logger.Information("Adding Brain {name}", name);
        _brains.Add(name, action);
    }
}
