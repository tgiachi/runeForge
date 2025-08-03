using Runeforge.Data.Context;

namespace Runeforge.Data.Entities.Npcs;

public class JsonNpcInventory
{
    public Dictionary<string, string> Items { get; set; } = new();

}
