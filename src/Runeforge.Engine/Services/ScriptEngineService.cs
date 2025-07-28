using DryIoc;
using MoonSharp.Interpreter;
using Runeforge.Core.Directories;
using Runeforge.Core.Types;
using Runeforge.Engine.Data.Configs.Services;
using Runeforge.Engine.Data.Internal.Scripts;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Utils;
using Serilog;

namespace Runeforge.Engine.Services;

public class ScriptEngineService : IScriptEngineService
{
    private readonly Script _globalScript;
    private readonly ScriptEngineConfig _scriptEngineConfig;

    private readonly List<Type> _scriptModules = new List<Type>();

    private readonly IContainer _container;

    private readonly ILogger _logger = Log.ForContext<ScriptEngineService>();

    private readonly Dictionary<string, Script> _loadedScripts = new();
    private readonly Dictionary<string, DateTime> _scriptModifiedTimes = new();
    private readonly DirectoriesConfig _directoriesConfig;


    public List<ScriptFunctionDescriptor> Functions { get; } = [];
    public List<Type> Enums { get; } = [];

    private FileSystemWatcher _fileSystemWatcher;


    public ScriptEngineService(
        DirectoriesConfig directoriesConfig, ScriptEngineConfig scriptEngineConfig, IContainer container
    )
    {
        _directoriesConfig = directoriesConfig;
        _scriptEngineConfig = scriptEngineConfig;
        _container = container;
        _globalScript = new Script();

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
            foreach (var global in _globalScript.Globals.Pairs)
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
    }


    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        InitFileWatcher();

        EmmyLuaDefinitionGenerator.GenerateDefinitionFile(
            Functions,
            Path.Combine(_scriptEngineConfig.DefinitionPath, "runeforge.lua"),
            Enums
        );
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

        var scanResult = ScriptDescriptorScanner.ScanClass(moduleType);

        if (scanResult.Count == 0)
        {
            _logger.Warning("No script functions found in module: {ModuleType}", moduleType.Name);
            return;
        }

        _scriptModules.Add(moduleType);
        _logger.Information("Registered script module: {ModuleType}", moduleType.Name);

        Functions.AddRange(scanResult);

        _globalScript.Globals[moduleType.Name] = _container.Resolve(moduleType);
    }
}
