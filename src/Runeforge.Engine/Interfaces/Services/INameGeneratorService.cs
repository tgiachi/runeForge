using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface INameGeneratorService : IRuneforgeService
{
    void AddName(string type, string name);

    string GenerateName(string type);

    List<string> Types { get; }
}
