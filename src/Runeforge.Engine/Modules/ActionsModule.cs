using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Modules;

[ScriptModule("actions")]
public class ActionsModule
{
    private readonly IActionService _actionService;

    public ActionsModule(IActionService actionService)
    {
        _actionService = actionService;
    }

    [ScriptFunction("Add new action")]
    public void AddAction(string name, Action<object> action)
    {
        _actionService.AddAction(name, action);
    }

    [ScriptFunction("Execute action")]
    public void ExecuteAction(string name, object? parameter = null)
    {
        _actionService.ExecuteAction(name, parameter);
    }
}
