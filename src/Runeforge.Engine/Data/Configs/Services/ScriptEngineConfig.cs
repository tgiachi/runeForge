namespace Runeforge.Engine.Data.Configs.Services;

public class ScriptEngineConfig
{
    public string DefinitionPath { get; set; }

    public List<string> StartupScripts { get; set; } = ["bootstrap.lua", "main.lua", "init.lua"];
}
