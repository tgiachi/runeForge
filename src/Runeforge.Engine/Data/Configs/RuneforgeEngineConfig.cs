using Runeforge.Engine.Data.Configs.Sections;

namespace Runeforge.Engine.Data.Configs;

public class RuneforgeEngineConfig
{
    public string GameName { get; set; } = "Runeforge Game";
    public string GameVersion { get; set; } = "1.0.0";
    public GameWindowConfig GameWindow { get; set; } = new();
}
