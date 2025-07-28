using DryIoc;
using Runeforge.Engine.Data.Internal.Services;

namespace Runeforge.Engine.Extensions;

public static class AddScriptModuleExtension
{
    public static IContainer AddScriptModule(this IContainer container, Type scriptModule)
    {
        if (scriptModule == null)
        {
            throw new ArgumentNullException(nameof(scriptModule), "Script module type cannot be null.");
        }


        container.AddToRegisterTypedList(new ScriptDefObject(scriptModule));

        container.Register(scriptModule, Reuse.Singleton);


        return container;
    }
}
