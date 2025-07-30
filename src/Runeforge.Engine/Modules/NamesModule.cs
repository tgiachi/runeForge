using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Modules;

[ScriptModule("names")]
public class NamesModule
{
    private readonly INameGeneratorService _nameGeneratorService;

    public NamesModule(INameGeneratorService nameGeneratorService)
    {
        _nameGeneratorService = nameGeneratorService;
    }

    [ScriptFunction("Generate new name")]
    public string GenerateName(string type = null)
    {
        return _nameGeneratorService.GenerateName(type);
    }
}
