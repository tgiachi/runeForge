using Runeforge.Data.Entities.Npcs;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.DataLoaders;

public class NpcDataLoader : IDataLoader
{
    private readonly INpcService _npcService;

    public NpcDataLoader(INpcService npcService)
    {
        _npcService = npcService;
    }

    public async Task LoadDataAsync(object data, CancellationToken cancellationToken = default)
    {
        if (data is JsonNpcData npc)
        {
            _npcService.AddNpc(npc);
        }
    }
}
