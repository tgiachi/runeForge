using Runeforge.Data.Entities.MapGen;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapGeneratorService: IRuneforgeService
{
    void AddStep(string name, Type generatorType);

    Task ExecuteGenerationAsync(string name);

    void AddMapGenerator(JsonMapGenData generator);
}
