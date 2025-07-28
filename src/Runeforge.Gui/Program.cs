using Runeforge.Engine.Bootstrap;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.Logger.Sink;
using Runeforge.Engine.Types;
using Runeforge.Gui;
using Runeforge.UI.Screens;
using SadConsole.Configuration;
using Serilog.Events;


var bootstrap = new RuneforgeBootstrap(
    new RuneforgeOptions()
    {
        RootDirectory = "/tmp/runeforge",
        LogLevel = LogLevelType.Debug,
        GameName = "test-game",
        LogToConsole = true,
        LogToFile = true
    }
);


Settings.WindowTitle = "My SadConsole Game";

Builder gameStartup = new Builder()
        .SetScreenSize(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT)
        .SetStartingScreen(host =>
            {
                var logViewer = new LogViewerScreen(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                bootstrap.OnLogEvent += entry => { logViewer.AddLogEntry(entry); };
                return logViewer;
            }
        )
        .IsStartingScreenFocused(true)
        .ConfigureFonts(true)
        .OnStart(StartBootstrap)
        .OnEnd(OnEnd)
    ;

void OnEnd(object? sender, GameHost e)
{
    bootstrap.StopAsync();
}

void StartBootstrap(object? sender, GameHost e)
{
    bootstrap.StartAsync();
}

Game.Create(gameStartup);
Game.Instance.Run();
Game.Instance.Dispose();
