using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using DryIoc;
using Jint;
using Jint.Native;
using Jint.Runtime.Interop;
using Runeforge.Core.Directories;
using Runeforge.Core.Extensions.Strings;
using Runeforge.Core.Types;
using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.Data.Configs.Services;
using Runeforge.Engine.Data.Internal.Scripts;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Types.Scripts;
using Runeforge.Engine.Utils;
using Serilog;

namespace Runeforge.Engine.Services;

public class JsScriptEngineService : IScriptEngineService
{
    private readonly ILogger _logger = Log.ForContext<JsScriptEngineService>();

    private readonly List<string> _initScripts;

    private readonly DirectoriesConfig _directoriesConfig;
    private readonly Jint.Engine _jsEngine;
    private readonly IContainer _serviceProvider;
    private readonly List<ScriptModuleData> _scriptModules;

    private readonly Dictionary<string, Action<object[]>> _callbacks = new();
    private readonly Dictionary<string, object> _constants = new();

    private readonly ScriptEngineConfig _scriptEngineConfig;

    private readonly IVersionService _versionService;


    private Func<string, string> _nameResolver;

    public JsScriptEngineService(
        DirectoriesConfig directoriesConfig,
        ScriptEngineConfig scriptEngineConfig,
        List<ScriptModuleData> scriptModules,
        IVersionService versionService, IContainer serviceProvider
    )
    {
        _scriptModules = scriptModules;
        _directoriesConfig = directoriesConfig;

        _scriptEngineConfig = scriptEngineConfig;

        _versionService = versionService;
        _serviceProvider = serviceProvider;

        _initScripts = _scriptEngineConfig.InitScriptsFileNames;

        CreateNameResolver();

        var typeResolver = TypeResolver.Default;
        typeResolver.MemberNameCreator = MemberNameCreator;
        _jsEngine = new Jint.Engine(options =>
            {
                options.EnableModules(directoriesConfig[DirectoryType.Scripts]);
                options.AllowClr(GetType().Assembly);
                options.SetTypeResolver(typeResolver);
            }
        );
    }

    private void CreateNameResolver()
    {
        _nameResolver = name => name.ToSnakeCase();

        _nameResolver = _scriptEngineConfig.ScriptNameConversion switch
        {
            ScriptNameConversion.CamelCase  => name => name.ToCamelCase(),
            ScriptNameConversion.PascalCase => name => name.ToPascalCase(),
            ScriptNameConversion.SnakeCase  => name => name.ToSnakeCase(),
            _                               => _nameResolver
        };
    }

    private IEnumerable<string> MemberNameCreator(MemberInfo memberInfo)
    {
        var memberType = _nameResolver(memberInfo.Name);
        _logger.Verbose("[JS] Creating member name  {MemberInfo}", memberType);
        yield return memberType;
    }


    private void ExecuteBootstrap()
    {
        foreach (var file in _initScripts.Select(s => Path.Combine(_directoriesConfig[DirectoryType.Scripts], s)))
        {
            if (File.Exists(file))
            {
                var fileName = Path.GetFileName(file);
                _logger.Information("Executing {FileName} script", fileName);
                ExecuteScriptFile(file);
            }
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2111", Justification = "Required delegate is referenced explicitly.")]
    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var module in _scriptModules)
        {
            var scriptModuleAttribute = module.ModuleType.GetCustomAttribute<ScriptModuleAttribute>();

            if (!_serviceProvider.IsRegistered(module.ModuleType))
            {
                _serviceProvider.Register(module.ModuleType);
            }

            var instance = _serviceProvider.GetService(module.ModuleType);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Unable to create instance of script module {module.ModuleType.Name}"
                );
            }

            _logger.Debug("Registering script module {Name}", scriptModuleAttribute.Name);

            _jsEngine.SetValue(scriptModuleAttribute.Name, instance);
        }

        AddConstant("version", _versionService.GetVersionInfo().Version);

        _logger.Debug("Generating scripts documentation in scripts directory named 'index.d.ts'");
        var documentation = TypeScriptDocumentationGenerator.GenerateDocumentation(
            "Run",
            _versionService.GetVersionInfo().Version,
            _scriptModules,
            _constants,
            _nameResolver
        );


        var enumsFound = TypeScriptDocumentationGenerator.FoundEnums;

        foreach (var enumFound in enumsFound)
        {
            _jsEngine.SetValue(
                _nameResolver.Invoke(enumFound.Name),
                TypeReference.CreateTypeReference(_jsEngine, enumFound)
            );
        }

        var definitionPath = Path.Combine(
            _directoriesConfig.Root,
            _scriptEngineConfig.DefinitionPath,
            "index.d.ts"
        );
        File.WriteAllText(definitionPath, documentation);


        _jsEngine.SetValue("importSync", RequireModule);
        _jsEngine.SetValue("require", RequireModule);

        _jsEngine.SetValue(
            "delay",
            new Func<int, Task>(async milliseconds => { await Task.Delay(milliseconds); })
        );

        ExecuteBootstrap();


        return Task.CompletedTask;
    }

    private JsValue RequireModule(string moduleName)
    {
        if (!moduleName.EndsWith(".js"))
        {
            moduleName += ".js";
        }

        var moduleNamespace = _jsEngine.Modules.Import(moduleName);

        return moduleNamespace;
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void AddInitScript(string script)
    {
        if (string.IsNullOrWhiteSpace(script))
        {
            throw new ArgumentException("Script cannot be null or empty", nameof(script));
        }

        _initScripts.Add(script);
    }

    public void ExecuteScript(string script)
    {
        try
        {
            _jsEngine.Execute(script);
        }
        catch (Exception e)
        {
            _logger.Error(e, "Error executing script");
        }
    }

    public void ExecuteScriptFile(string scriptFile)
    {
        var content = File.ReadAllText(scriptFile);

        ExecuteScript(content);
    }

    public void AddCallback(string name, Action<object[]> callback)
    {
        _callbacks[name] = callback;
    }

    public void AddConstant(string name, object value)
    {
        _constants[name.ToSnakeCaseUpper()] = value;
        _jsEngine.SetValue(name.ToSnakeCaseUpper(), value);
    }

    public void ExecuteCallback(string name, params object[] args)
    {
        if (_callbacks.TryGetValue(name, out var callback))
        {
            _logger.Debug("Executing callback {Name}", name);
            callback(args);
        }
        else
        {
            _logger.Warning("Callback {Name} not found", name);
        }
    }

    public void AddScriptModule(Type type)
    {
        _scriptModules.Add(new ScriptModuleData(type));
    }

    public string ToScriptEngineFunctionName(string name)
    {
        return _nameResolver.Invoke(name);
    }

    public void Dispose()
    {
        _jsEngine.Dispose();
    }
}
