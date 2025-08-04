using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IMapGeneratorService: IRuneforgeService
{
    void AddStep(string name, IMapGenerator generator);

    Task ExecuteGenerationAsync(string name);
}
