using MoonSharp.Interpreter;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Maps;

namespace Runeforge.Engine.Maps.Generators.Base;

public class FuncMapGenerator : IMapGeneratorStep
{
    private readonly DynValue _generatorFunction;

    public FuncMapGenerator(DynValue generatorFunction)
    {
        _generatorFunction = generatorFunction;
    }

    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        var result = _generatorFunction.Function.Call(context);

        return result.ToObject<MapGeneratorContext>();
    }
}
