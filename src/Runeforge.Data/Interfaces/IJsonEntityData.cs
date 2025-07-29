namespace Runeforge.Data.Interfaces;

public interface IJsonEntityData
{
    string Id { get; set; }

    string Name { get; set; }

    List<string> Tags { get; set; }
}
