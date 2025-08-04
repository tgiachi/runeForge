using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Maps;

namespace Runeforge.Engine.Maps.Generators.Base;

public class FuncMapGenerator : IMapGeneratorStep
{

    private readonly Func<MapGeneratorContext, MapGeneratorContext> _func;

    public FuncMapGenerator(Func<MapGeneratorContext, MapGeneratorContext> generatorFunction)
    {
        _func = generatorFunction;
    }

    public async Task<MapGeneratorContext> GenerateMapAsync(MapGeneratorContext context)
    {
        var result = _func.Invoke(context);

        return result;
    }
}
