using System.Reflection;
using DryIoc;
using MoonSharp.Interpreter;
using Runeforge.Core.Directories;
using Runeforge.Core.Types;
using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Data.Configs.Services;
using Runeforge.Engine.Data.Internal.Scripts;
using Runeforge.Engine.Data.Internal.Services;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Utils;
using Serilog;

namespace Runeforge.Engine.Services;

public class ScriptEngineService : IScriptEngineService
{
    private readonly ScriptEngineConfig _scriptEngineConfig;

    private readonly List<Type> _scriptModules = new List<Type>();

    private readonly IContainer _container;

    private readonly ILogger _logger = Log.ForContext<ScriptEngineService>();

    private readonly Dictionary<string, Script> _loadedScripts = new();
    private readonly Dictionary<string, DateTime> _scriptModifiedTimes = new();
    private readonly List<ScriptDefObject> _scriptDefObjects;
    private readonly DirectoriesConfig _directoriesConfig;

    private readonly Dictionary<string, DynValue> _globalModules = new();

    public List<ScriptFunctionDescriptor> Functions { get; } = [];
    public List<Type> Enums { get; } = [];

    private FileSystemWatcher _fileSystemWatcher;


    public ScriptEngineService(
        DirectoriesConfig directoriesConfig, ScriptEngineConfig scriptEngineConfig, IContainer container,
        List<ScriptDefObject> scriptDefObjects
    )
    {
        _directoriesConfig = directoriesConfig;
        _scriptEngineConfig = scriptEngineConfig;
        _scriptDefObjects = scriptDefObjects;
        _container = container;

        RegisterGlobalBindings();
    }

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
    /// Load a specific script file
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
        foreach (var scriptDef in _scriptDefObjects)
        {
            AddScriptModule(scriptDef.ModuleType);
        }
    }

    /// <summary>
    /// Load all Lua scripts from the scripts directory
    /// </summary>
    public async Task LoadAllScripts()
    {
        var scriptsPath = _directoriesConfig[DirectoryType.Scripts];

        if (!Directory.Exists(scriptsPath))
        {
            _logger.Warning("Scripts directory not found: {Path}", scriptsPath);
            return;
        }

        var luaFiles = Directory.GetFiles(scriptsPath, "*.lua", SearchOption.AllDirectories);

        foreach (var filePath in luaFiles)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            if (fileName.StartsWith("__") || fileName.StartsWith("runeforge_"))
            {
                _logger.Debug("Skipping internal script: {FilePath}", filePath);
                continue; // Skip internal scripts
            }

            await LoadScript(filePath);
        }
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        InitFileWatcher();

        EmmyLuaDefinitionGenerator.GenerateDefinitionFile(
            Functions,
            Path.Combine(_scriptEngineConfig.DefinitionPath, "__runeforge.lua"),
            "0.0.1",
            Enums
        );

        LoadAllScripts();
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
}
