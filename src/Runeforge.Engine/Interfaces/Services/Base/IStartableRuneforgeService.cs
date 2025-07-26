namespace Runeforge.Engine.Interfaces.Services.Base;

public interface IStartableRuneforgeService : IRuneforgeService
{
    Task StartAsync(CancellationToken cancellationToken = default);

    Task StopAsync(CancellationToken cancellationToken = default);
}
