using DryIoc;
using Runeforge.Data.Entities.MapGen;
using Runeforge.Engine.Contexts;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.Interfaces.Maps;
using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class MapGeneratorService : IMapGeneratorService
{
    private readonly ILogger _logger = Log.ForContext<MapGeneratorService>();

    private readonly List<JsonMapGenData> _mapGenData = new();

    private readonly Dictionary<string, IMapGeneratorStep> _generatorsSteps = new();


    private readonly IContainer _container;

    public MapGeneratorService(IContainer container)
    {
        _container = container;
    }

    public void AddStep(string name, Type generatorType)
    {
        if (!_container.IsRegistered(generatorType))
        {
            _container.Register(generatorType, Reuse.Singleton);
        }

        if (_container.Resolve(generatorType) is not IMapGeneratorStep generator)
        {
            _logger.Error("Failed to resolve generator of type {GeneratorType}", generatorType);
            return;
        }

        var mapGenerator = _container.Resolve(generatorType) as IMapGeneratorStep;

        _generatorsSteps.Add(name, mapGenerator);
    }

    public void AddStep(string name, IMapGeneratorStep generator)
    {
        if (generator == null)
        {
            _logger.Error("Generator cannot be null");
            return;
        }

        if (_generatorsSteps.ContainsKey(name))
        {
            _logger.Warning("Generator step with name {Name} already exists, replacing it", name);
        }

        _generatorsSteps[name] = generator;
        _logger.Information("Added map generator step {Name}", name);
    }

    public async Task ExecuteGenerationAsync(string name)
    {
        var mapGen = _mapGenData.FirstOrDefault(x => x.Id == name);

        if (mapGen == null)
        {
            _logger.Error("Map generator with name {Name} not found", name);
            return;
        }

        var map = new GameMap(mapGen.Width, mapGen.Height, null);


        var stepContext = new MapGeneratorContext(map);

        foreach (var stepValue in mapGen.Steps)
        {
            stepContext.Name = stepValue.StepName;

            var step = _generatorsSteps.GetValueOrDefault(stepValue.StepName);
            stepContext = await step.GenerateMapAsync(stepContext);


            stepContext.Step++;
        }
    }

    public Task ExecuteDefaultGenerationAsync()
    {
        _logger.Information("Executing default map generation");

        if (_mapGenData.Count == 0)
        {
            _logger.Error("No map generators available");
            return Task.CompletedTask;
        }

        var defaultGen = _mapGenData.FirstOrDefault(s => s.IsDefault);

        if (defaultGen == null)
        {
            defaultGen = _mapGenData.FirstOrDefault();
        }

        return ExecuteGenerationAsync(defaultGen.Id);
    }

    public void AddMapGenerator(JsonMapGenData generator)
    {
        _logger.Information("Adding map generator {Generator}", generator.Id);
        _mapGenData.Add(generator);
    }
}
