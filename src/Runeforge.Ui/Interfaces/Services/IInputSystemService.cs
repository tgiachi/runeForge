using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Ui.Interfaces.Services;

public interface IInputSystemService : IRuneforgeService
{
    string Context { get; set; }

    void AddKeyBinding(string context, string key, string actionName);
}
