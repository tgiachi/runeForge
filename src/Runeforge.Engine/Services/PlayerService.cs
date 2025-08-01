using Runeforge.Engine.GameObjects;
using Runeforge.Engine.Interfaces.Services;

namespace Runeforge.Engine.Services;

public class PlayerService : IPlayerService
{
    public PlayerGameObject Player { get; set; }
}
