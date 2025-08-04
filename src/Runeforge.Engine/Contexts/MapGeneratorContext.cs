using GoRogue.GameFramework;

namespace Runeforge.Engine.Contexts;

public class MapGeneratorContext
{
    public int Step { get; set; }
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<string, object> Outputs { get; set; }
    public Dictionary<string, object> Inputs { get; set; }

    public MapGeneratorContext()
    {
        Outputs = new Dictionary<string, object>();
        Inputs = new Dictionary<string, object>();
    }
}

