using Runeforge.Engine.Bootstrap;
using Runeforge.Engine.Extensions;
using Runeforge.Ui.Interfaces.Services;
using Runeforge.Ui.Services;

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

}
