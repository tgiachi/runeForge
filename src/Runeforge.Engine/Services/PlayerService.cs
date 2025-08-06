using GoRogue.GameFramework;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Interfaces.Services;
using SadRogue.Primitives;

namespace Runeforge.Engine.Services;

public class PlayerService : IPlayerService
{
    public PlayerGameObject Player { get; set; } = null!;

    private readonly ITileSetService _tileSetService;

    public PlayerService(ITileSetService tileSetService)
    {
        _tileSetService = tileSetService;
    }

    public void CreatePlayer(string tile = "player")
    {
        if (Player == null)
        {
            var tileColored = _tileSetService.CreateGlyph(tile);
            Player = new PlayerGameObject(Point.None, tileColored.ColoredGlyph)
            {
                Name = "Player",
            };

            Player.AddedToMap += PlayerOnAddedToMap;
        }
    }

    private void PlayerOnAddedToMap(object? sender, GameObjectCurrentMapChanged e)
    {
        Player.GoRogueComponents.Add(new PlayerFOVController());
    }

    public void UpdateFov()
    {
        Player.AllComponents.GetFirstOrDefault<PlayerFOVController>()?.CalculateFOV();
    }
}
