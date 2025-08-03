using Runeforge.Engine.Contexts;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IAiService : IRuneforgeService
{
    void AddBrain(string name, Action<AiContext> action);
}
