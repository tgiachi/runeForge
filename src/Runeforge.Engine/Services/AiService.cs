using Runeforge.Engine.Contexts;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.TickActions;
using Serilog;

namespace Runeforge.Engine.Services;

public class AiService : IAiService
{
    private readonly Dictionary<string, Action<AiContext>> _brains = new();

    private readonly ILogger _logger = Log.ForContext<AiService>();

    private readonly IMapService _mapService;
    private readonly IPlayerService _playerService;
    private readonly ITickSystemService _tickSystemService;

    private readonly List<AiComponent> _aiComponents = new();


    public AiService(IMapService mapService, IPlayerService playerService, ITickSystemService tickSystemService)
    {
        _mapService = mapService;
        _playerService = playerService;
        _tickSystemService = tickSystemService;

        _mapService.NpcAdded += OnNpcAdded;

        //_tickSystemService.TickEnded += OnTick;
    }

    // private void OnTick(int tickCount)
    // {
    //     foreach (var aiComponent in _aiComponents)
    //     {
    //         if (aiComponent.AiContext == null)
    //         {
    //             _logger.Warning("AiContext is null for npc {npc}", aiComponent.AiContext.Self.Name);
    //             continue;
    //         }
    //
    //         if (_brains.TryGetValue(aiComponent.BrainName, out var brainAction))
    //         {
    //             try
    //             {
    //                 brainAction(aiComponent.AiContext);
    //             }
    //             catch (Exception ex)
    //             {
    //                 _logger.Error(
    //                     ex,
    //                     "Error executing brain {brain} for npc {npc}",
    //                     aiComponent.BrainName,
    //                     aiComponent.AiContext.Self.Name
    //                 );
    //             }
    //         }
    //     }
    // }

    private void OnNpcAdded(NpcGameObject entity)
    {
        _logger.Information("Adding npc {npc}", entity.Name);

        var aiComponent = entity.AllComponents.GetFirstOrDefault<AiComponent>();

        if (aiComponent != null)
        {
            _logger.Information("Bring to life npc {npc} with brain {brain}", entity.Name, aiComponent.BrainName);

            aiComponent.AiContext = AiContext.Create(entity, _playerService.Player);

            _aiComponents.Add(aiComponent);

           var action = _brains[aiComponent.BrainName];

           _tickSystemService.EnqueueAction(new AiProcessAction(aiComponent, action));
        }
    }

    public void AddBrain(string name, Action<AiContext> action)
    {
        _logger.Information("Adding Brain {name}", name);
        _brains.Add(name, action);
    }
}
