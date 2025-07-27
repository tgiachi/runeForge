using DryIoc;
using Runeforge.Core.Directories;
using Runeforge.Core.Extensions.Strings;
using Runeforge.Core.Types;
using Runeforge.Engine.Data.Internal.Services;
using Runeforge.Engine.Data.Options;
using Runeforge.Engine.Extensions;
using Runeforge.Engine.Extensions.Loggers;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Interfaces.Services.Base;
using Runeforge.Engine.Logger.Sink;
using Runeforge.Engine.Services;
using Serilog;

namespace Runeforge.Engine.Bootstrap;

public class RuneforgeBootstrap
{
    private readonly IContainer _container = new Container(rules => rules.WithoutThrowOnRegisteringDisposableTransient());

    private readonly CancellationTokenRegistration _cancellationTokenRegistration = new();

    private readonly RuneforgeOptions _runeforgeOptions;

    private DirectoriesConfig _directoriesConfig;

    public RuneforgeBootstrap(RuneforgeOptions options)
    {
        _runeforgeOptions = options ?? throw new ArgumentNullException(nameof(options));

        InitializeRootDirectory();
        InitializeLogger();
        RegisterServices();
    }


    private void InitializeRootDirectory()
    {
        Console.WriteLine("Root directory is: {0}", _runeforgeOptions.RootDirectory);
        _directoriesConfig = new DirectoriesConfig(_runeforgeOptions.RootDirectory, Enum.GetNames<DirectoryType>());

        _container.RegisterInstance(_directoriesConfig);
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
            _runeforgeOptions.LogEventDelegate,
            _runeforgeOptions.LogLevel.ToSerilogLogLevel()
        );


        Log.Logger = loggerConfiguration.CreateLogger();

        Log.Logger.Information("Runeforge logger initialized.");
    }

    public async Task StartAsync()
    {
        await StartStopServiceAsync(true);
    }

    private async Task StartStopServiceAsync(bool isStart)
    {
        var servicesDef = _container.Resolve<List<ServiceDefObject>>().OrderBy(s => s.Priority).ToList();


        foreach (var serviceDef in servicesDef)
        {
            _container.Resolve(serviceDef.ServiceType);
            Log.Logger.Debug("Ctor service: {ServiceType}", serviceDef.ImplementationType);
        }

        foreach (var serviceDef in servicesDef)
        {
            var serviceInstance = _container.Resolve(serviceDef.ServiceType);
            if (serviceInstance is IStartableRuneforgeService startableService)
            {
                if (isStart)
                {
                    Log.Logger.Debug("Starting service: {ServiceType}", serviceDef.ImplementationType);
                    await startableService.StartAsync(_cancellationTokenRegistration.Token);
                }
                else
                {
                    Log.Logger.Debug("Stopping service: {ServiceType}", serviceDef.ImplementationType);
                    await startableService.StopAsync(_cancellationTokenRegistration.Token);
                }
            }
        }
    }


    private async Task StopAsync()
    {
        await StartStopServiceAsync(false);
        await _cancellationTokenRegistration.DisposeAsync();
        Log.Logger.Information("Runeforge services stopped.");
    }

    private void RegisterServices()
    {
        _container.RegisterService(typeof(IEventBusService), typeof(EventBusService));
    }
}
