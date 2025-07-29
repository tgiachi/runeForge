using Runeforge.Engine.Data.Internal.Scripts;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IScriptEngineService : IRuneforgeStartableService
{
    List<ScriptFunctionDescriptor> Functions { get; }

    List<Type> Enums { get; }

    void AddScriptModule(Type moduleType);

    void AddEnum<TEnum>() where TEnum : Enum;
}
