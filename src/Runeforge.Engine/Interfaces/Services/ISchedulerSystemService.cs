using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface ISchedulerSystemService : IRuneforgeService
{
    Task RegisterJob(string name, Func<Task> task, TimeSpan interval);
    Task UnregisterJob(string name);
    Task<bool> IsJobRegistered(string name);
    Task PauseJob(string name);
    Task ResumeJob(string name);
}
