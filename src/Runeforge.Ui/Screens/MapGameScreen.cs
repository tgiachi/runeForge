using GoRogue.MapGeneration;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.GameObjects;
using Runeforge.Engine.GameObjects.Components;
using Runeforge.Engine.Instance;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Ui.Instances;
using Runeforge.Ui.Screens.Base;
using Runeforge.Ui.Utils;
using SadConsole;
using SadConsole.Components;
using SadConsole.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Console = SadConsole.Console;

namespace Runeforge.UI.Screens;

public class MapGameScreen : BaseRuneforgeScreenSurface
{
    public SurfaceComponentFollowTarget ViewLock;

    private readonly IPlayerService _playerService;
    private readonly IMapService _mapService;
    private readonly ITileSetService _tileSetService;

    private readonly Console _textConsole;

    public MapGameScreen(int width, int height) : base(width, height)
    {
        _mapService = RuneforgeInstances.GetService<IMapService>();
        _playerService = RuneforgeInstances.GetService<IPlayerService>();
        _tileSetService = RuneforgeInstances.GetService<ITileSetService>();

        _textConsole = new Console(width, height);

        _textConsole.Font = RuneforgeGuiInstance.Instance.DefaultUiFont;
        _textConsole.IsVisible = true;
        _textConsole.IsFocused = false;
        _textConsole.UseKeyboard = false;
        _textConsole.Clear();


        _mapService.MapChanged += MapServiceOnMapChanged;
        _mapService.MapGenerated += MapServiceOnMapGenerated;
        _mapService.StartGenerateDefaultMapAsync();


        // mapService.MapGenerated += MapServiceOnMapGenerated;

        // var viewport = ViewportUtils.CalculateViewport(
        //     new Point(width, height),
        //     RuneforgeGuiInstance.Instance.DefaultUiFont.GlyphWidth,
        //     RuneforgeGuiInstance.Instance.DefaultUiFont.GlyphHeight,
        //     RuneforgeGuiInstance.Instance.DefaultMapFont.GlyphWidth,
        //     RuneforgeGuiInstance.Instance.DefaultMapFont.GlyphHeight
        // );
        //
        // var mapGeneratorService = RuneforgeInstances.GetService<IMapGeneratorService>();
        //
        // mapGeneratorService.ExecuteDefaultGenerationAsync().GetAwaiter().GetResult();
        //
        // var mapId = mapService.GenerateMapAsync(300, 300, "Test", "Test Map").GetAwaiter().GetResult();
        //
        // var mapObjectInfo = mapService.GetMapInfo(mapId);
        //
        // var currentMap = mapObjectInfo?.Map;
        //
        // currentMap.DefaultRenderer = currentMap.CreateRenderer(
        //     viewport,
        //     RuneforgeGuiInstance.Instance.DefaultMapFont
        // );
        //
        // Children.Add(currentMap);
        //
        //
        // var tileSetService = RuneforgeInstances.GetService<ITileSetService>();
        //
        // var playerService = RuneforgeInstances.GetService<IPlayerService>();
        //
        // var playerColoredGlyph = tileSetService.CreateGlyph("player");
        //
        // playerService.Player = new PlayerGameObject(new Point(30, 30), playerColoredGlyph.ColoredGlyph);
        //
        // playerService.Player.GoRogueComponents.Add(new PlayerFOVController());
        //
        // currentMap.AddEntity(playerService.Player);
        //
        // ViewLock = new SurfaceComponentFollowTarget() { Target = playerService.Player };
        // currentMap.DefaultRenderer.SadComponents.Add(ViewLock);
        // playerService.Player.AllComponents.GetFirstOrDefault<PlayerFOVController>().CalculateFOV();
        // IsFocused = true;
        // UseKeyboard = true;
    }

    private void MapServiceOnMapChanged(MapInfoObject oldMap, MapInfoObject newMap)
    {
        Children.Clear();

        var currentMap = newMap?.Map;

        var viewport = ViewportUtils.CalculateViewport(
            new Point(Width, Height),
            RuneforgeGuiInstance.Instance.DefaultUiFont.GlyphWidth,
            RuneforgeGuiInstance.Instance.DefaultUiFont.GlyphHeight,
            RuneforgeGuiInstance.Instance.DefaultMapFont.GlyphWidth,
            RuneforgeGuiInstance.Instance.DefaultMapFont.GlyphHeight
        );

        currentMap.DefaultRenderer = currentMap.CreateRenderer(
            viewport,
            RuneforgeGuiInstance.Instance.DefaultMapFont
        );




        Children.Add(currentMap);

        Children.Add(_textConsole);



        //

        ViewLock = new SurfaceComponentFollowTarget() { Target = _playerService.Player };
        currentMap.DefaultRenderer.SadComponents.Add(ViewLock);
        _playerService.UpdateFov();
        //playerService.Player.AllComponents.GetFirstOrDefault<PlayerFOVController>().CalculateFOV();
        IsFocused = true;
        UseKeyboard = true;
    }

    private Task MapServiceOnMapGenerated(MapInfoObject mapInfo)
    {
        _mapService.CurrentMap = mapInfo;
        return Task.CompletedTask;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        var actionService = RuneforgeInstances.GetService<IActionService>();
        var playerService = RuneforgeInstances.GetService<IPlayerService>();
        var Player = playerService.Player;
        if (keyboard.IsKeyPressed(Keys.W))
        {
            actionService.ExecuteAction("move_up", Player);
        }

        if (keyboard.IsKeyPressed(Keys.S))
        {
            actionService.ExecuteAction("move_down", Player);
        }

        if (keyboard.IsKeyPressed(Keys.A))
        {
            actionService.ExecuteAction("move_left", Player);
        }

        if (keyboard.IsKeyPressed(Keys.D))
        {
            actionService.ExecuteAction("move_right", Player);
        }


        if (keyboard.IsKeyPressed(Keys.Space))
        {
            actionService.ExecuteAction("execute_tick");
        }

        if (keyboard.IsKeyPressed(Keys.F2))
        {
            var itemService = RuneforgeInstances.GetService<IItemService>();
            var mapService = RuneforgeInstances.GetService<IMapService>();

            var potion = itemService.CreateItemGameObject("i_simple_potion");

            potion.Position = Player.Position + Direction.Up;

            mapService.AddEntityInCurrentMap(potion);
        }

        if (keyboard.IsKeyPressed(Keys.F3))
        {
            var npcService = RuneforgeInstances.GetService<INpcService>();
            var mapService = RuneforgeInstances.GetService<IMapService>();

            var npc = npcService.CreateNpcGameObject("a_orion");

            npc.Position = Player.Position + Direction.Down;

            mapService.AddEntityInCurrentMap(npc);
        }

        if (keyboard.IsKeyPressed(Keys.F4))
        {
            var npcService = RuneforgeInstances.GetService<INpcService>();
            var mapService = RuneforgeInstances.GetService<IMapService>();

            foreach (var i in Enumerable.Range(1, 150))
            {
                var randomPosition = new Point(
                    Random.Shared.Next(0, mapService.CurrentMap.Map.Width),
                    Random.Shared.Next(0, mapService.CurrentMap.Map.Height)
                );

                var npc = npcService.CreateNpcGameObject("a_orion");

                if (mapService.CurrentMap.Map.CanAddEntityAt(npc, randomPosition))
                {
                    npc.Position = randomPosition;

                    mapService.AddEntityInCurrentMap(npc);
                }
            }
        }

        Player.UpdateFOV();

        return base.ProcessKeyboard(keyboard);
    }
}
