using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Services.Base;

public abstract class BaseStartableRuneforgeService<TService> : BaseRuneforgeService<TService>, IStartableRuneforgeService
    where TService : BaseStartableRuneforgeService<TService>
{
    public virtual Task StartAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public virtual Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
