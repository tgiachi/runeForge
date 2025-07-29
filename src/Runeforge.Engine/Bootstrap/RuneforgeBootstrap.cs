using DryIoc;
using Runeforge.Core.Directories;
using Runeforge.Core.Extensions.Strings;
using Runeforge.Core.Resources;
using Runeforge.Core.Types;
using Runeforge.Engine.Data.Configs.Services;
using Runeforge.Engine.Data.Events.Engine;
using Runeforge.Engine.Data.Internal.Services;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.Extensions;
using Runeforge.Engine.Extensions.Loggers;
using Runeforge.Engine.Instance;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Interfaces.Services.Base;
using Runeforge.Engine.Logger.Sink;
using Runeforge.Engine.Modules;
using Runeforge.Engine.Services;
using Serilog;

namespace Runeforge.Engine.Bootstrap;

public class RuneforgeBootstrap
{
    public delegate void RegisterServicesDelegate(IContainer container);

    public event RegisterServicesDelegate OnRegisterServices;

    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new();
    private readonly IContainer _container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

    private readonly RuneforgeOptions _runeforgeOptions;

    private DirectoriesConfig _directoriesConfig;


    public RuneforgeBootstrap(RuneforgeOptions options)
    {
        _runeforgeOptions = options ?? throw new ArgumentNullException(nameof(options));

        RuneforgeInstances.Container = _container;

        InitializeRootDirectory();
        InitializeLogger();
    }

    //  public RuneforgeSink.LogEventDelegate LogEventDelegate { get; set; }

    public event RuneforgeSink.LogEventDelegate OnLogEvent;

    public void Initialize()
    {
        PrintHeader();
        RegisterServices();
        RegisterScriptModules();
        OnRegisterServices?.Invoke(_container);
    }

    private static void PrintHeader()
    {
        var header = ResourceUtils.GetEmbeddedResourceContent("Assets/header.txt", typeof(RuneforgeBootstrap).Assembly);

        Console.WriteLine(header);
    }


    private void InitializeRootDirectory()
    {
        Console.WriteLine("Root directory is: {0}", _runeforgeOptions.RootDirectory);
        _directoriesConfig = new DirectoriesConfig(_runeforgeOptions.RootDirectory, Enum.GetNames<DirectoryType>());

        _container.RegisterInstance(_directoriesConfig);
    }


    private void RegisterScriptModules()
    {
        _container
            .AddScriptModule(typeof(LoggerModule))
            .AddScriptModule(typeof(ActionsModule))
            .AddScriptModule(typeof(RandomModule))
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
                Path.Combine(_directoriesConfig[DirectoryType.Logs], $"{_runeforgeOptions.GameName.ToSnakeCase()}_.log"),
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
            var serviceInstance = _container.Resolve(serviceDef.ServiceType);
            if (serviceInstance is IRuneforgeStartableService startableService)
            {
                if (isStart)
                {
                    Log.Logger.Debug("Starting service: {ServiceType}", serviceDef.ImplementationType.Name);
                    await startableService.StartAsync(_cancellationTokenRegistration.Token);
                }
                else
                {
                    Log.Logger.Debug("Stopping service: {ServiceType}", serviceDef.ImplementationType.Name);
                    await startableService.StopAsync(_cancellationTokenRegistration.Token);
                }
            }
        }
    }


    public async Task StopAsync()
    {
        await StartStopServiceAsync(false);
        await _cancellationTokenRegistration.DisposeAsync();
        Log.Logger.Information("Runeforge services stopped.");
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
            ;

        // Register Configs
        _container.RegisterInstance(
            new DiagnosticServiceConfig
            {
                PidFileName = $"{_runeforgeOptions.GameName.ToSnakeCase()}.pid",
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
