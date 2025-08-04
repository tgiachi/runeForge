using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Contexts;
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
    public void AddStep(string name, Func<MapGeneratorContext, MapGeneratorContext> func)
    {
        var generator = new FuncMapGenerator(func);
        _mapGeneratorService.AddStep(name, generator);
    }
}
