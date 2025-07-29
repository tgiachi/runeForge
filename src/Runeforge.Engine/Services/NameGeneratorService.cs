using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class NameGeneratorService : INameGeneratorService
{
    private readonly ILogger _logger = Log.ForContext<NameGeneratorService>();

    private readonly Dictionary<string, List<string>> _names = new();

    public void AddName(string type, string name)
    {
        _logger.Verbose("Adding name '{Name}' to type '{Type}'.", name, type);
        if (!_names.TryGetValue(type, out var names))
        {
            names = [];
            _names.Add(type, names);
        }

        names.Add(name);
    }

    public string GenerateName(string type) => _names.TryGetValue(type, out var names)
        ? names[Random.Shared.Next(0, names.Count)]
        : string.Empty;
}
