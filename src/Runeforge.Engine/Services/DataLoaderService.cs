using DryIoc;
using Humanizer;
using Runeforge.Core.Directories;
using Runeforge.Core.Json;
using Runeforge.Core.Types;
using Runeforge.Data.Entities.Base;
using Runeforge.Engine.Interfaces.DataLoaders;
using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class DataLoaderService : IDataLoaderService
{
    private readonly IContainer _container;

    private readonly Dictionary<Type, IDataLoader> _dataLoaders = new();

    private readonly DirectoriesConfig _directoriesConfig;
    private readonly ILogger _logger = Log.ForContext<DataLoaderService>();

    public DataLoaderService(DirectoriesConfig directoriesConfig, IContainer container)
    {
        _directoriesConfig = directoriesConfig;
        _container = container;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        LoadTemplatesAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public void AddDataLoader<TDataLoader, TDataType>() where TDataLoader : IDataLoader where TDataType : class
    {
        if (!_container.IsRegistered<TDataLoader>())
        {
            _logger.Debug(
                "Registering data loader {DataLoader} for type {DataType}",
                typeof(TDataLoader).Name,
                typeof(TDataType).Name
            );
            _container.Register<TDataLoader>(Reuse.Singleton);
        }

        var dataLoader = _container.Resolve<TDataLoader>();
        var dataType = typeof(TDataType);

        _dataLoaders[dataType] = dataLoader;
    }


    private async Task LoadTemplatesAsync()
    {
        var files = Directory.GetFiles(
            Path.Combine(_directoriesConfig[DirectoryType.Templates]),
            "*.json",
            SearchOption.AllDirectories
        );

        foreach (var file in files)
        {
            await LoadTemplateAsync(file);
        }
    }

    private async Task LoadTemplateAsync(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var length = new FileInfo(filePath).Length;
        _logger.Information("Loading template from {FilePath} ({Bytes})", fileName, length.Bytes());

        try
        {
            var jsonData = JsonUtils.DeserializeFromFile<BaseJsonEntityData[]>(filePath);

            foreach (var entity in jsonData)
            {
                var entityType = entity.GetType();

                if (_dataLoaders.TryGetValue(entityType, out var dataLoader))
                {
                    _logger.Verbose("Using existing data loader for {EntityType}", entityType.Name);
                    await dataLoader.LoadDataAsync(entity, CancellationToken.None);
                }
                else
                {
                    _logger.Warning("No data loader found for {EntityType}", entityType.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error loading template from {FilePath}", fileName);
            throw new InvalidOperationException($"Failed to load template from {fileName}", ex);
        }
    }
}
