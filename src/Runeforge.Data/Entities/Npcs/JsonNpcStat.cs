namespace Runeforge.Data.Entities.Npcs;

public class JsonNpcStat
{
    // We can use rnd(1, 20) to generate random values for these stats
    public string Strength { get; set; }
    public string Dexterity { get; set; }
    public string Intelligence { get; set; }

    public string Constitution { get; set; }
    public string Wisdom { get; set; }
    public string Charisma { get; set; }
}
