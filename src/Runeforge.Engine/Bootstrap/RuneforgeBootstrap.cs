using DryIoc;
using Runeforge.Core.Directories;
using Runeforge.Core.Extensions.Strings;
using Runeforge.Core.Json;
using Runeforge.Core.Resources;
using Runeforge.Core.Types;
using Runeforge.Data.Entities.Names;
using Runeforge.Engine.Data.Configs;
using Runeforge.Engine.Data.Configs.Services;
using Runeforge.Engine.Data.Events.Engine;
using Runeforge.Engine.Data.Internal.Services;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.DataLoaders;
using Runeforge.Engine.Extensions;
using Runeforge.Engine.Extensions.Loggers;
using Runeforge.Engine.Instance;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Interfaces.Services.Base;
using Runeforge.Engine.Json;
using Runeforge.Engine.Logger.Sink;
using Runeforge.Engine.Modules;
using Runeforge.Engine.Services;
using SadConsole;
using Serilog;
using Console = System.Console;

namespace Runeforge.Engine.Bootstrap;

public class RuneforgeBootstrap
{
    public delegate void RegisterServicesDelegate(IContainer container);

    public event RegisterServicesDelegate OnRegisterServices;

    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new();
    private readonly IContainer _container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

    private readonly RuneforgeOptions _runeforgeOptions;

    public RuneforgeEngineConfig EngineConfig { get; }

    public string GameTitle => $"{EngineConfig.GameName} - {EngineConfig.GameVersion}";

    private DirectoriesConfig _directoriesConfig;


    public RuneforgeBootstrap(RuneforgeOptions options)
    {
        JsonUtils.RegisterJsonContext(RuneforgeJsonContext.Default);

        _runeforgeOptions = options ?? throw new ArgumentNullException(nameof(options));
        RuneforgeInstances.Container = _container;


        InitializeRootDirectory();
        EngineConfig = InitializeConfig(options.ConfigName);
        InitializeLogger();
    }



    private RuneforgeEngineConfig InitializeConfig(string configName)
    {
        Console.WriteLine($"Loading config: {configName}");
        var config = new RuneforgeEngineConfig();

        var configPath = Path.Combine(_directoriesConfig.Root, configName);

        if (!File.Exists(configPath))
        {
            JsonUtils.SerializeToFile(config, configPath);
        }

        config = JsonUtils.DeserializeFromFile<RuneforgeEngineConfig>(
            configPath
        );

        JsonUtils.SerializeToFile(config, configPath);


        return config;
    }


    //  public RuneforgeSink.LogEventDelegate LogEventDelegate { get; set; }

    public event RuneforgeSink.LogEventDelegate OnLogEvent;

    public void Initialize()
    {
        PrintHeader();
        RegisterServices();
        RegisterScriptModules();
        RegisterDefaultDataLoaders();
        OnRegisterServices?.Invoke(_container);
    }

    private void RegisterDefaultDataLoaders()
    {
        var dataLoaderService = _container.Resolve<IDataLoaderService>();
        dataLoaderService.AddDataLoader<NamesDataLoader, JsonNameData>();
    }

    private static void PrintHeader()
    {
        var header = ResourceUtils.GetEmbeddedResourceContent("Assets/_header.txt", typeof(RuneforgeBootstrap).Assembly);

        Console.WriteLine(header);
    }


    private void InitializeRootDirectory()
    {
        Console.WriteLine("Root directory is: {0}", _runeforgeOptions.RootDirectory);
        _directoriesConfig = new DirectoriesConfig(_runeforgeOptions.RootDirectory, Enum.GetNames<DirectoryType>());

        CopyAssetsFilesAsync(_directoriesConfig);

        _container.RegisterInstance(_directoriesConfig);
    }


    private void RegisterScriptModules()
    {
        _container
            .AddScriptModule(typeof(LoggerModule))
            .AddScriptModule(typeof(ActionsModule))
            .AddScriptModule(typeof(RandomModule))
            .AddScriptModule(typeof(NamesModule))
            ;
    }

    private void InitializeLogger()
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(_runeforgeOptions.LogLevel.ToSerilogLogLevel())
            .Enrich.FromLogContext();

        if (_runeforgeOptions.LogToConsole)
        {
            loggerConfiguration = loggerConfiguration.WriteTo.Console();
        }

        if (_runeforgeOptions.LogToFile)
        {
            loggerConfiguration.WriteTo.File(
                Path.Combine(_directoriesConfig[DirectoryType.Logs], $"{EngineConfig.GameName.ToSnakeCase()}_.log"),
                rollingInterval: RollingInterval.Day
            );
        }

        loggerConfiguration.WriteTo.Delegate(
            entry => { OnLogEvent?.Invoke(entry); },
            _runeforgeOptions.LogLevel.ToSerilogLogLevel()
        );


        Log.Logger = loggerConfiguration.CreateLogger();

        Log.Logger.Information("Runeforge logger initialized.");
    }

    public async Task StartAsync()
    {
        await StartStopServiceAsync(true);
        var eventBusService = _container.Resolve<IEventBusService>();
        await eventBusService.PublishAsync(new EngineStartedEvent());
    }

    private async Task StartStopServiceAsync(bool isStart)
    {
        var servicesDef = _container.Resolve<List<ServiceDefObject>>().OrderBy(s => s.Priority).ToList();


        foreach (var serviceDef in servicesDef)
        {
            _container.Resolve(serviceDef.ServiceType);
            Log.Logger.Debug("Ctor service: {ServiceType}", serviceDef.ImplementationType.Name);
        }

        foreach (var serviceDef in servicesDef)
        {
            try
            {
                var serviceInstance = _container.Resolve(serviceDef.ServiceType);
                if (serviceInstance is IRuneforgeStartableService startableService)
                {
                    if (isStart)
                    {
                        Log.Logger.Debug("Starting service: {ServiceType}", serviceDef.ImplementationType.Name);
                        await startableService.StartAsync();
                    }
                    else
                    {
                        Log.Logger.Debug("Stopping service: {ServiceType}", serviceDef.ImplementationType.Name);
                        await startableService.StopAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(
                    ex,
                    "Error while {Action} service: {ServiceType}",
                    isStart ? "starting" : "stopping",
                    serviceDef.ImplementationType.Name
                );
                throw new InvalidOperationException(
                    $"Failed to {(isStart ? "start" : "stop")} service: {serviceDef.ImplementationType.Name}",
                    ex
                );
            }
        }

        Log.Logger.Information("Runeforge services {Action}ed successfully.", isStart ? "start" : "stop");
    }


    public async Task StopAsync()
    {
        await StartStopServiceAsync(false);
        await _cancellationTokenRegistration.DisposeAsync();
        Log.Logger.Information("Runeforge services stopped.");
    }

    static async Task CopyAssetsFilesAsync(DirectoriesConfig directoriesConfig)
    {
        var assets = ResourceUtils.GetEmbeddedResourceNames(typeof(RuneforgeBootstrap).Assembly, "Assets");
        var files = assets.Select(s => new
                { Asset = s, FileName = ResourceUtils.ConvertResourceNameToPath(s, "Runeforge.Engine.Assets") }
            )
            .ToList();


        foreach (var assetFile in files)
        {
            var fileName = Path.Combine(directoriesConfig.Root, assetFile.FileName);

            if (assetFile.FileName.StartsWith("_"))
            {
                continue;
            }

            if (!File.Exists(fileName))
            {
                Log.Logger.Information("Copying asset {FileName}", fileName);

                var content = ResourceUtils.GetEmbeddedResourceContent(assetFile.Asset, typeof(RuneforgeBootstrap).Assembly);

                var directory = Path.GetDirectoryName(fileName);

                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(fileName, content);
            }
        }
    }


    private void RegisterServices()
    {
        _container
            .RegisterService(typeof(IVersionService), typeof(VersionService))
            .RegisterService(typeof(IEventBusService), typeof(EventBusService))
            .RegisterService(typeof(ISchedulerSystemService), typeof(SchedulerSystemService))
            .RegisterService(typeof(IDiagnosticService), typeof(DiagnosticService))
            .RegisterService(typeof(IEventDispatcherService), typeof(EventDispatcherService))
            .RegisterService(typeof(IScriptEngineService), typeof(ScriptEngineService))
            .RegisterService(typeof(IDataLoaderService), typeof(DataLoaderService))
            .RegisterService(typeof(IActionService), typeof(ActionService))

            //
            .RegisterService(typeof(INameGeneratorService), typeof(NameGeneratorService))
            .RegisterService(typeof(IVariablesService), typeof(VariableService))
            .RegisterService(typeof(IMapService), typeof(MapService))
            ;

        // Register Configs
        _container.RegisterInstance(
            new DiagnosticServiceConfig
            {
                PidFileName = $"{EngineConfig.GameName.ToSnakeCase()}.pid",
                MetricsIntervalInSeconds = 60
            }
        );

        _container.RegisterInstance(
            new ScriptEngineConfig
            {
                DefinitionPath = _directoriesConfig[DirectoryType.Scripts]
            }
        );
    }
}
