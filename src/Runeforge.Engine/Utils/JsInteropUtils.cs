using System.Diagnostics.CodeAnalysis;
using Jint;
using Jint.Native;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Utils;

public static class JsInteropUtils
{
    public static bool ImplementsInterface<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)] T>(
        JsValue jsValue, IScriptEngineService scriptEngineService
    )
    {
        if (!jsValue.IsObject())
        {
            return false;
        }

        var obj = jsValue.AsObject();
        var type = typeof(T);

        foreach (var method in type.GetMethods())
        {
            var jsProp = obj.Get(scriptEngineService.ToScriptEngineFunctionName(method.Name));
            if (jsProp is JsUndefined)
            {
                throw new InvalidOperationException(
                    $"The JavaScript object does not implement the method '{method.Name}' required by the interface '{type.Name}'."
                );
            }
        }

        return true;
    }


}
