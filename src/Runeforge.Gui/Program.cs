using System.Globalization;
using ConsoleAppFramework;
using Runeforge.Core.Json;
using Runeforge.Data.Context;
using Runeforge.Engine.Bootstrap;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.Types.Logger;
using Runeforge.Ui.Extensions;
using Runeforge.UI.Screens;
using SadConsole.Configuration;



JsonUtils.RegisterJsonContext(JsonEntityContext.Default);
ConsoleApp.Run(
    args,
    (
        string rootDirectory = "", LogLevelType levelType = LogLevelType.Debug, bool logToConsole = true,
        bool logToFile = true
    ) =>
    {
        LoadApp(rootDirectory, levelType, logToConsole, logToFile);
    }
);

static void LoadApp(string rootDirectory, LogLevelType levelType, bool logToConsole, bool logToFile)
{
    if (string.IsNullOrWhiteSpace(rootDirectory))
    {
        rootDirectory = Environment.GetEnvironmentVariable("RUNEFORGE_ROOT_DIRECTORY") ??
                        Path.Combine(Directory.GetCurrentDirectory(), "Runeforge");
    }

    var bootstrap = new RuneforgeBootstrap(
        new RuneforgeOptions
        {
            RootDirectory = rootDirectory,
            LogLevel = levelType,
            LogToConsole = logToConsole,
            LogToFile = logToFile,
        }
    );

    bootstrap.RegisterUiServices();


    Settings.WindowTitle = bootstrap.GameTitle;


    var gameStartup = new Builder()
            .SetScreenSize(bootstrap.EngineConfig.GameWindow.Width, bootstrap.EngineConfig.GameWindow.Height)
            .SetStartingScreen(host =>
                {
                    var logViewer = new LogViewerScreen(
                        bootstrap.EngineConfig.GameWindow.Width,
                        bootstrap.EngineConfig.GameWindow.Height
                    );
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

        bootstrap.Initialize();
        _ = Task.Run(async () =>
        {
            await bootstrap.StartAsync();
            bootstrap.InitGuiInstance(GameHost.Instance);
        });

    }



    Game.Create(gameStartup);
    Game.Instance.Run();
    Game.Instance.Dispose();
}
