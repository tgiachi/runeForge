using Runeforge.Data.Interfaces;

namespace Runeforge.Data.Entities.Base;

public class BaseJsonEntityData : IJsonEntityData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Tags { get; set; } = new();
}
