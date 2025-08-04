using Jint;
using Jint.Native;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Wraps.Base;

public class BaseWrap
{
    protected JsValue Value { get; set; }

    private readonly IScriptEngineService _scriptEngineService;

    public BaseWrap(IScriptEngineService scriptEngineService, JsValue jsValue)
    {
        _scriptEngineService = scriptEngineService;
        Value = jsValue;
    }

    protected JsValue Call(string methodName, params object[] args)
    {
        var method = Value.Get(_scriptEngineService.ToScriptEngineFunctionName(methodName));

        return Value.AsObject()
            .Engine.Call(
                method,
                Value.AsObject(),
                args.Select(arg => JsValue.FromObject(Value.AsObject().Engine, arg)).ToArray()
            );
    }

    public TOut Call<TOut>(string methodName, params object[] args)
    {
        return Call(methodName, args).ToObject() is TOut result ? result : default!;
    }
}
