using Runeforge.Data.Entities.Items;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IItemService : IRuneforgeService
{
    void AddItem(JsonItemData item);

    ItemGameObject CreateItemGameObject(string idCategoryTag);

}
