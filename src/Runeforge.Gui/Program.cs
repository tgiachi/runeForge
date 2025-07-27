using Runeforge.Engine.Bootstrap;
using Runeforge.Engine.Data.Options;
using Runeforge.Gui;
using SadConsole.Configuration;

var bootstrap = new RuneforgeBootstrap(new RuneforgeOptions());

Settings.WindowTitle = "My SadConsole Game";

Builder gameStartup = new Builder()
        .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
        .SetStartingScreen<RootScreen>()
        .IsStartingScreenFocused(true)
        .ConfigureFonts(true)
        .OnStart(StartBootstrap)
    ;

void StartBootstrap(object? sender, GameHost e)
{
    bootstrap.StartAsync();
}

Game.Create(gameStartup);
Game.Instance.Run();
Game.Instance.Dispose();
