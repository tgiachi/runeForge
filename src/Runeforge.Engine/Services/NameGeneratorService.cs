using Runeforge.Engine.Interfaces.Services;
using Serilog;

namespace Runeforge.Engine.Services;

public class NameGeneratorService : INameGeneratorService
{
    private readonly ILogger _logger = Log.ForContext<NameGeneratorService>();

    private readonly Dictionary<string, List<string>> _names = new();
    public List<string> Types => _names.Keys.ToList();

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

    public string GenerateName(string type)
    {
        if (string.IsNullOrWhiteSpace(type))
        {
            type = Types[Random.Shared.Next(0, Types.Count)];
        }

        if (_names.TryGetValue(type, out var names) && names.Count > 0)
        {
            var name = names[Random.Shared.Next(0, names.Count)];
            return name;
        }

        _logger.Warning("No names found for type '{Type}'. Returning empty string.", type);

        return string.Empty;
    }
}
