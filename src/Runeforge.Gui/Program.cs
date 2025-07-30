using System.Globalization;
using ConsoleAppFramework;
using Runeforge.Core.Directories;
using Runeforge.Core.Json;
using Runeforge.Core.Types;
using Runeforge.Data.Context;
using Runeforge.Engine.Bootstrap;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.Instance;
using Runeforge.Engine.Types.Logger;
using Runeforge.Ui.Extensions;
using Runeforge.UI.Screens;
using SadConsole.Configuration;
using SadConsole.Input;
using Serilog;


JsonUtils.RegisterJsonContext(JsonEntityContext.Default);
ConsoleApp.Run(
    args,
    (
        string rootDirectory = "", LogLevelType levelType = LogLevelType.Debug, bool logToConsole = true,
        bool logToFile = true, bool enableDebugger = false
    ) =>
    {
        LoadApp(rootDirectory, levelType, logToConsole, logToFile, enableDebugger);
    }
);

static void LoadApp(string rootDirectory, LogLevelType levelType, bool logToConsole, bool logToFile, bool enableDebugger)
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
        .ConfigureFonts((f, g) =>
            {
                var directoriesConfig = RuneforgeInstances.GetService<DirectoriesConfig>();
                var allFonts = Directory.GetFiles(directoriesConfig[DirectoryType.Fonts], "*.font");
                foreach (var fontFile in allFonts)
                {
                    Log.Logger.Information("Loading font: {FontFile}", fontFile);
                    try
                    {
                        var font = g.LoadFont(fontFile);
                        g.Fonts[font.Name] = font;
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "Failed to load font: {FontFile}", fontFile);
                    }
                }
            }
        );

    if (enableDebugger)
    {
        gameStartup.EnableImGuiDebugger(Keys.F12);
    }

    gameStartup.OnStart(StartBootstrap)
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
            }
        );
    }


    Game.Create(gameStartup);
    Game.Instance.Run();
    Game.Instance.Dispose();
}
