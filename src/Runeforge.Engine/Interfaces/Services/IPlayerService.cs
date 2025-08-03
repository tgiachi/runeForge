using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface IPlayerService : IRuneforgeService
{
    PlayerGameObject Player { get; set; }
}
