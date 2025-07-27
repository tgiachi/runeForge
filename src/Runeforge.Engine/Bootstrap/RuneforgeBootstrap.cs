using DryIoc;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.Extensions;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Services;

namespace Runeforge.Engine.Bootstrap;

public class RuneforgeBootstrap
{
    private readonly IContainer _container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

    public RuneforgeBootstrap(RuneforgeOptions options)
    {
        RegisterServices();
    }


    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    private Task StopAsync()
    {
        return Task.CompletedTask;
    }

    private void RegisterServices()
    {
        _container.RegisterService(typeof(IEventBusService), typeof(EventBusService));
    }
}
