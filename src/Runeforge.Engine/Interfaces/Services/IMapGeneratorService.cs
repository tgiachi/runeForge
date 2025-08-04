using Runeforge.Data.Entities.MapGen;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapGeneratorService: IRuneforgeService
{
    void AddStep(string name, Type generatorType);

    void AddStep(string name, IMapGeneratorStep generator);

    Task ExecuteGenerationAsync(string name);

    Task ExecuteDefaultGenerationAsync();

    void AddMapGenerator(JsonMapGenData generator);
}
