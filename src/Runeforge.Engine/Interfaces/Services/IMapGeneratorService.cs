using Runeforge.Data.Entities.MapGen;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapGeneratorService: IRuneforgeService
{
    void AddStep(string name, Type generatorType);

    void AddStep(string name, IMapGeneratorStep generator);

    Task<GameMap> ExecuteGenerationAsync(string name, string mapId = "");

    Task<GameMap> ExecuteDefaultGenerationAsync(string mapId = "");

    void AddMapGenerator(JsonMapGenData generator);

    string GetDefaultGeneratorName();
}
