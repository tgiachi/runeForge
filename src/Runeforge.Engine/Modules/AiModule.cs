using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Modules;

[ScriptModule("ai")]
public class AiModule
{
    private readonly IAiService _aiService;

    public AiModule(IAiService aiService)
    {
        _aiService = aiService;
    }

    [ScriptFunction("Add brain")]
    public void AddBrain(string name, Action<AiContext> action)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Brain name cannot be null or whitespace.", nameof(name));
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action), "Action cannot be null.");
        }

        _aiService.AddBrain(name, action);
    }
}
