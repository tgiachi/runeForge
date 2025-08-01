using Runeforge.Engine.Contexts;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class AiService : IAiService
{
    private readonly Dictionary<string, Action<AiContext>> _brains = new();

    private readonly ILogger _logger = Log.ForContext<AiService>();


    private readonly IMapService _mapService;
    private readonly IPlayerService _playerService;

    public AiService(IMapService mapService, IPlayerService playerService)
    {
        _mapService = mapService;
        _playerService = playerService;

        _mapService.NpcAdded += OnNpcAdded;
    }

    private void OnNpcAdded(NpcGameObject entity)
    {
        _logger.Information("Adding npc {npc}", entity.Name);

        var aiComponent = entity.AllComponents.GetFirstOrDefault<AiComponent>();

        if (aiComponent != null)
        {
            _logger.Information("Bring to life npc {npc} with brain {brain}", entity.Name, aiComponent.BrainName);

            aiComponent.AiContext = AiContext.Create(entity, _playerService.Player);
        }
    }

    public void AddBrain(string name, Action<AiContext> action)
    {
        _logger.Information("Adding Brain {name}", name);
        _brains.Add(name, action);
    }
}
