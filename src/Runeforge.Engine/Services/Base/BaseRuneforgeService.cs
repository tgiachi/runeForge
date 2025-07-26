using Runeforge.Engine.Interfaces.Services.Base;
using Serilog;

namespace Runeforge.Engine.Services.Base;

public abstract class BaseRuneforgeService<TService> : IRuneforgeService
{
    protected readonly ILogger _logger = Log.ForContext<TService>();


}
