using System.Reflection;
using DryIoc;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Loaders;
using Runeforge.Core.Directories;
using Runeforge.Core.Types;
using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.Data.Configs.Services;
using Runeforge.Engine.Data.Events.Engine;
using Runeforge.Engine.Data.Internal.Scripts;
using Runeforge.Engine.Data.Internal.Services;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Utils;
using Serilog;

namespace Runeforge.Engine.Services;

public class ScriptEngineService : IScriptEngineService
{
    private readonly IContainer _container;
    private readonly IEventBusService _eventBusService;

    private readonly DirectoriesConfig _directoriesConfig;

    private readonly Dictionary<string, DynValue> _globalModules = new();

    private readonly Dictionary<string, Script> _loadedScripts = new();


    private readonly ILogger _logger = Log.ForContext<ScriptEngineService>();
    private readonly List<ScriptDefObject> _scriptDefObjects;
    private readonly ScriptEngineConfig _scriptEngineConfig;
    private readonly Dictionary<string, DateTime> _scriptModifiedTimes = new();

    private readonly List<Type> _scriptModules = new();

    private FileSystemWatcher _fileSystemWatcher;


    public ScriptEngineService(
        DirectoriesConfig directoriesConfig, ScriptEngineConfig scriptEngineConfig, IContainer container,
        List<ScriptDefObject> scriptDefObjects, IEventBusService eventBusService
    )
    {
        _directoriesConfig = directoriesConfig;
        _scriptEngineConfig = scriptEngineConfig;
        _scriptDefObjects = scriptDefObjects;
        _eventBusService = eventBusService;
        _container = container;

        RegisterGlobalBindings();

        _eventBusService.Subscribe<EngineStartedEvent>(OnEngineStarted);
    }

    private async Task OnEngineStarted(EngineStartedEvent obj)
    {
        ExecuteBootstrap();
    }

    public List<ScriptFunctionDescriptor> Functions { get; } = [];
    public List<Type> Enums { get; } = [];

    public void AddEnum<TEnum>() where TEnum : Enum
    {
        if (!Enums.Contains(typeof(TEnum)))
        {
            UserData.RegisterType<TEnum>();

            Enums.Add(typeof(TEnum));
            _logger.Information("Registered enum: {EnumType}", typeof(TEnum).Name);
        }
        else
        {
            _logger.Warning("Enum {EnumType} is already registered", typeof(TEnum).Name);
        }
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        EmmyLuaDefinitionGenerator.GenerateDefinitionFile(
            Functions,
            Path.Combine(_scriptEngineConfig.DefinitionPath, "__runeforge.lua"),
            "0.0.1",
            Enums
        );

        await LoadInitScript();

        InitFileWatcher();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void AddScriptModule(Type moduleType)
    {
        if (!_container.IsRegistered(moduleType))
        {
            _container.Register(moduleType, Reuse.Singleton);
        }

        var moduleName = moduleType.GetCustomAttribute<ScriptModuleAttribute>().Name;

        var scanResult = ScriptDescriptorScanner.ScanClass(moduleType);

        if (scanResult.Count == 0)
        {
            _logger.Warning("No script functions found in module: {ModuleType}", moduleType.Name);
            return;
        }

        _scriptModules.Add(moduleType);
        _logger.Information("Registered script module: {ModuleType}", moduleType.Name);

        Functions.AddRange(scanResult);

        UserData.RegisterType(moduleType);

        var instance = _container.Resolve(moduleType);
        var dynValue = UserData.Create(instance);

        _globalModules[moduleName] = dynValue;
    }

    private void InitFileWatcher()
    {
        _fileSystemWatcher = new FileSystemWatcher(_directoriesConfig[DirectoryType.Scripts])
        {
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.lua",
            EnableRaisingEvents = true
        };

        _fileSystemWatcher.Created += OnScriptChanged;
        _fileSystemWatcher.Changed += OnScriptChanged;

        _logger.Information(
            "File watcher initialized for scripts directory: {Path}",
            _directoriesConfig[DirectoryType.Scripts]
        );
    }

    /// <summary>
    ///     Load a specific script file
    /// </summary>
    public async Task<bool> LoadScript(string filePath)
    {
        try
        {
            var scriptName = Path.GetFileNameWithoutExtension(filePath);
            var scriptContent = await File.ReadAllTextAsync(filePath);

            var script = new Script();

            // Copy global bindings to new script
            foreach (var global in _globalModules)
            {
                script.Globals[global.Key] = global.Value;
            }

            var fileLoader = new FileSystemScriptLoader();
            script.Options.ScriptLoader = fileLoader;
            fileLoader.ModulePaths =
            [
                Path.Combine(_directoriesConfig[DirectoryType.Scripts], "?.lua"),
                Path.Combine(_directoriesConfig[DirectoryType.Scripts], "?/init.lua"),
                Path.Combine(_directoriesConfig[DirectoryType.Scripts], "/?.lua"),
                Path.Combine(_directoriesConfig[DirectoryType.Scripts], "/?/init.lua")
            ];


            // Execute script
            script.DoString(scriptContent);

            _loadedScripts[scriptName] = script;
            _scriptModifiedTimes[filePath] = File.GetLastWriteTime(filePath);

            _logger.Debug("Loaded script: {ScriptName}", scriptName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to load script: {FilePath}", filePath);
            return false;
        }
    }

    private void ExecuteBootstrap()
    {
        // Execute bootstrap logic if needed
        _logger.Information("Executing script engine bootstrap logic...");

        var bootstrapExecuted = false;

        foreach (var (name, script) in _loadedScripts)
        {
            try
            {
                var bootstrap = script.Globals.Get("bootstrap");

                if (bootstrap.IsNil())
                {
                    _logger.Debug("No bootstrap function found in script: {ScriptName}", name);
                    continue;
                }

                script.Call(bootstrap);

                bootstrapExecuted = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error executing bootstrap for script: {ScriptName}", name);
            }
        }

        if (!bootstrapExecuted)
        {
            throw new Exception(
                "No bootstrap function found in any loaded script. Please ensure at least one script defines a 'bootstrap' function."
            );
        }
    }

    private async void OnScriptChanged(object sender, FileSystemEventArgs e)
    {
        try
        {
            var fileName = Path.GetFileNameWithoutExtension(e.FullPath);

            if (fileName.StartsWith("__") || fileName.StartsWith("runeforge_"))
            {
                _logger.Debug("Skipping internal script change event: {FilePath}", e.FullPath);
                return; // Skip internal scripts
            }

            if (!_scriptEngineConfig.StartupScripts.Contains(fileName))
            {
                _logger.Debug("Skipping non-startup script change event: {FilePath}", e.FullPath);
                return;
            }

            // Debounce multiple events
            await Task.Delay(100);

            var currentModified = File.GetLastWriteTime(e.FullPath);

            if (_scriptModifiedTimes.TryGetValue(e.FullPath, out var lastModified) &&
                currentModified <= lastModified)
            {
                return; // No actual change
            }

            _logger.Information("Script file changed, reloading: {FilePath}", e.FullPath);
            await LoadScript(e.FullPath);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error handling script file change: {FilePath}", e.FullPath);
        }
    }


    private void RegisterGlobalBindings()
    {
        UserData.RegisterType<Action>();

        UserData.RegisterType<AiContext>();
        UserData.RegisterType<Action<AiContext>>();
        foreach (var scriptDef in _scriptDefObjects)
        {
            AddScriptModule(scriptDef.ModuleType);
        }
    }

    /// <summary>
    ///   Load initial scripts defined in the configuration
    /// </summary>
    public async Task LoadInitScript()
    {
        var scriptsPath = _directoriesConfig[DirectoryType.Scripts];


        if (!Directory.Exists(scriptsPath))
        {
            _logger.Warning("Scripts directory not found: {Path}", scriptsPath);
            return;
        }

        // var luaFiles = Directory.GetFiles(scriptsPath, "*.lua", SearchOption.AllDirectories);

        var initFileFound = false;
        foreach (var initFile in _scriptEngineConfig.StartupScripts)
        {
            var initFilePath = Path.Combine(scriptsPath, initFile);
            if (File.Exists(initFilePath))
            {
                _logger.Information("Loading startup script: {InitFile}", initFilePath);
                await LoadScript(initFilePath);
                initFileFound = true;
            }
            else
            {
                _logger.Warning("Startup script not found: {InitFile}", initFilePath);
            }
        }

        if (!initFileFound)
        {
            throw new Exception(
                "No startup scripts found in the configuration. Please check your script engine configuration."
            );
        }

        // foreach (var filePath in luaFiles)
        // {
        //     var fileName = Path.GetFileNameWithoutExtension(filePath);
        //     if (fileName.StartsWith("__") || fileName.StartsWith("runeforge_"))
        //     {
        //         _logger.Debug("Skipping internal script: {FilePath}", filePath);
        //         continue; // Skip internal scripts
        //     }
        //
        //     await LoadScript(filePath);
        // }
    }
}
