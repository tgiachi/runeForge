using MoonSharp.Interpreter;
using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Maps.Generators.Base;

namespace Runeforge.Engine.Modules;

[ScriptModule("mapGen")]
public class MapGenModule
{

    private readonly IMapGeneratorService _mapGeneratorService;

    public MapGenModule(IMapGeneratorService mapGeneratorService)
    {
        _mapGeneratorService = mapGeneratorService;
    }

    [ScriptFunction("add step to map generator")]
    public void AddStep(string name, DynValue genFunction)
    {
        if (genFunction.Type != DataType.Function)
        {
            throw new ScriptRuntimeException("Generator function must be a valid function");
        }

        var generator = new FuncMapGenerator(genFunction);
        _mapGeneratorService.AddStep(name, generator);
    }

}
