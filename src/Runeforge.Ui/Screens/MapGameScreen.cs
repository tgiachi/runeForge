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
using SadConsole.Effects;
using SadConsole.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Console = SadConsole.Console;

namespace Runeforge.UI.Screens;

public class MapGameScreen : BaseRuneforgeScreenSurface
{
    public readonly SurfaceComponentFollowTarget ViewLock;

    public PlayerGameObject Player { get; set; }

    public MapGameScreen(int width, int height) : base(width, height)
    {
        var mapService = RuneforgeInstances.GetService<IMapService>();
        var textConsole = new Console(width, height);
        textConsole.Font = RuneforgeGuiInstance.Instance.DefaultUiFont;
        textConsole.IsVisible = true;
        textConsole.IsFocused = false;
        textConsole.UseKeyboard = false;
        textConsole.Clear();


        mapService.MapGenerated += MapServiceOnMapGenerated;

        var viewport = ViewportUtils.CalculateViewport(
            new Point(width, height),
            RuneforgeGuiInstance.Instance.DefaultUiFont.GlyphWidth,
            RuneforgeGuiInstance.Instance.DefaultUiFont.GlyphHeight,
            RuneforgeGuiInstance.Instance.DefaultMapFont.GlyphWidth,
            RuneforgeGuiInstance.Instance.DefaultMapFont.GlyphHeight
        );

        var mapId = mapService.GenerateMapAsync(300, 300, "Test", "Test Map").GetAwaiter().GetResult();

        var mapObjectInfo = mapService.GetMapInfo(mapId);

        var currentMap = mapObjectInfo?.Map;

        currentMap.DefaultRenderer = currentMap.CreateRenderer(
            viewport,
            RuneforgeGuiInstance.Instance.DefaultMapFont
        );

        Children.Add(currentMap);
        Children.Add(textConsole);


        var tileSetService = RuneforgeInstances.GetService<ITileSetService>();

        var playerColoredGlyph = tileSetService.CreateGlyph("player");

        Player = new PlayerGameObject(new Point(30, 30), playerColoredGlyph.ColoredGlyph);

        Player.GoRogueComponents.Add(new PlayerFOVController());

        currentMap.AddEntity(Player);

        ViewLock = new SurfaceComponentFollowTarget() { Target = Player };
        currentMap.DefaultRenderer.SadComponents.Add(ViewLock);
        Player.AllComponents.GetFirstOrDefault<PlayerFOVController>().CalculateFOV();

        // var fov = new GoRogue.FOV.RecursiveShadowcastingFOV(currentMap.TransparencyView);
        //
        // var lightPos = new Point(10, 20);
        //
        // fov.Calculate(lightPos, 10);

        // foreach (var pos in currentMap.Positions())
        // {
        //     var bright = fov.DoubleResultView[pos];
        //
        //     var visible = fov.BooleanResultView[pos];
        //
        //     var cell = currentMap.TerrainView[pos.X, pos.Y];
        //     if (visible)
        //     {
        //         var fade = (float)Math.Max(0.3, bright);
        //         cell.Foreground = Color.White * fade;
        //     }
        //     else
        //     {
        //         cell.Foreground = Color.DarkGray;
        //     }
        // }

        // textConsole.PrintFadingText("Dio cano", new Point(Width / 2, Height / 2), TimeSpan.FromSeconds(1), new Blink());

        IsFocused = true;
        UseKeyboard = true;
    }

    private Task MapServiceOnMapGenerated(MapInfoObject mapInfo, Generator generator)
    {
        var tileSetService = RuneforgeInstances.GetService<ITileSetService>();

        var wallFloors = generator.Context.GetFirstOrDefault<ISettableGridView<bool>>("WallFloor");

        //var floorTile = new ColoredGlyph(Color.White, Color.Transparent, 3954);
        //var wallTile = new ColoredGlyph(Color.Gray, Color.Transparent, 2863);
        var floorTile = tileSetService.CreateGlyph("floor");
        var wallTile = tileSetService.CreateGlyph("wall");

        mapInfo.Map.ApplyTerrainOverlay(
            wallFloors,
            (point, val) => val
                ? new TerrainGameObject(point, tileSetService.CreateGlyph("floor").ColoredGlyph, "floor")
                : new TerrainGameObject(point, tileSetService.CreateGlyph("wall").ColoredGlyph, "wall", false)
        );

        return Task.CompletedTask;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (keyboard.IsKeyPressed(Keys.W))
        {
            Player.MoveTo(Direction.Up);
        }

        if (keyboard.IsKeyPressed(Keys.S))
        {
            Player.MoveTo(Direction.Down);
        }

        if (keyboard.IsKeyPressed(Keys.A))
        {
            Player.MoveTo(Direction.Left);
        }

        if (keyboard.IsKeyPressed(Keys.D))
        {
            Player.MoveTo(Direction.Right);
        }

        if (keyboard.IsKeyPressed(Keys.Space))
        {
            Player.ShowAllMap();
        }

        Player.UpdateFOV();

        return base.ProcessKeyboard(keyboard);
    }
}
