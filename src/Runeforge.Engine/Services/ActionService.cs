using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class ActionService : IActionService
{
    private readonly Dictionary<string, Action<object>> _actions = new();
    private readonly ILogger _logger = Log.ForContext<ActionService>();

    public void AddAction(string name, Action<object> action)
    {
        if (!_actions.TryAdd(name, action))
        {
            _logger.Warning("Action {ActionName} is already registered", name);
            return;
        }

        _logger.Information("Registered action: {ActionName}", name);
    }

    public void ExecuteAction(string name, object? parameter = null)
    {
        if (!_actions.TryGetValue(name, out var action))
        {
            _logger.Warning("Action {ActionName} is not registered", name);
            return;
        }

        try
        {
            _logger.Debug("Executing action: {ActionName}", name);
            action(parameter ?? new object());
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error executing action: {ActionName}", name);
        }
    }
}
