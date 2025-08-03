using Runeforge.Data.Entities.Npcs;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface INpcService : IRuneforgeService
{


    void AddNpc(JsonNpcData npc);

    NpcGameObject CreateNpcGameObject(string idCategoryTag);
}
