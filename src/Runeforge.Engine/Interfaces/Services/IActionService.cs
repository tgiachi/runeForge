using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IActionService : IRuneforgeService
{
    void AddAction(string name, Action<object> action);

    void ExecuteAction(string name, object? parameter = null);
}
