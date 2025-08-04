using GoRogue.GameFramework;
using Runeforge.Engine.GameObjects;
using SadRogue.Primitives;

namespace Runeforge.Engine.Contexts;

public class MapGeneratorContext
{
    public int Step { get; set; }
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<string, object> Outputs { get; set; }
    public Dictionary<string, object> Inputs { get; set; }

    public Map Map { get; set; }

    public MapGeneratorContext(Map map)
    {
        Outputs = new Dictionary<string, object>();
        Inputs = new Dictionary<string, object>();

        Map = map;
        Width = map.Width;
        Height = map.Height;
    }

    public void AddOutput(string key, object value)
    {
        Outputs[key] = value;
    }

    public object GetOutput(string key)
    {
        if (Outputs.TryGetValue(key, out var value))
        {
            return value;
        }

        throw new KeyNotFoundException($"Output with key '{key}' not found.");
    }

    public void ApplyTerrain(int x, int y, TerrainGameObject terrain)
    {
        terrain.Position = new Point(x, y);
        Map.SetTerrain(terrain);
    }
}
