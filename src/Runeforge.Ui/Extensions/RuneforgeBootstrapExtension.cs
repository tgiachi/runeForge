using Runeforge.Core.Directories;
using Runeforge.Core.Types;
using Runeforge.Engine.Bootstrap;
using Runeforge.Engine.Extensions;
using Runeforge.Engine.Instance;
using Runeforge.Ui.Instances;
using Runeforge.Ui.Interfaces.Services;
using Runeforge.Ui.Services;
using SadConsole;

namespace Runeforge.Ui.Extensions;

public static class RuneforgeBootstrapExtension
{
    public static RuneforgeBootstrap RegisterUiServices(this RuneforgeBootstrap bootstrap)
    {
        bootstrap.OnRegisterServices += container =>
        {
            container.RegisterService(typeof(IInputSystemService), typeof(InputSystemService));
        };

        return bootstrap;
    }

    public static void InitGuiInstance(this RuneforgeBootstrap bootstrap, GameHost gameHost)
    {
        var engineConfig = bootstrap.EngineConfig;

        if (string.IsNullOrEmpty(engineConfig.DefaultUiFont))
        {
            RuneforgeGuiInstance.Instance.DefaultUiFont = gameHost.Fonts[engineConfig.DefaultUiFont];
        }

        RuneforgeGuiInstance.Instance.DefaultUiFontSize = engineConfig.FontSize;
        RuneforgeGuiInstance.Instance.GameWindowConfig = engineConfig.GameWindow;
    }
}
