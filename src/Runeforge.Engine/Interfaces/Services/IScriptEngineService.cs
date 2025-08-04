using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IScriptEngineService: IRuneforgeStartableService
{
    void AddInitScript(string script);
    void ExecuteScript(string script);
    void ExecuteScriptFile(string scriptFile);
    void AddCallback(string name, Action<object[]> callback);
    void AddConstant(string name, object value);
    void ExecuteCallback(string name, params object[] args);
    void AddScriptModule(Type type);
    string ToScriptEngineFunctionName(string name);
}
