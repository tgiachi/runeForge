using DryIoc;
using Runeforge.Engine.Data.Options;

namespace Runeforge.Engine.Bootstrap;

public class RuneforgeBootstrap
{
    private readonly IContainer _container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

    public RuneforgeBootstrap(RuneforgeOptions options)
    {
    }


    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    private Task StopAsync()
    {
        return Task.CompletedTask;
    }
}
