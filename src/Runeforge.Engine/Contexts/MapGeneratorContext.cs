using System.Text.Json;
using GoRogue.GameFramework;
using Jint.Native;
using Runeforge.Engine.Data.Maps;
using Runeforge.Engine.GameObjects;
using SadRogue.Primitives;

namespace Runeforge.Engine.Contexts;

public class MapGeneratorContext
{
    private readonly Jint.Engine _engine;
    public int Step { get; set; }
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public Dictionary<string, object> Outputs { get; set; }
    public Dictionary<string, object> Inputs { get; set; }

    public GameMap Map { get; set; }

    public MapGeneratorContext(GameMap map, Jint.Engine engine)
    {
        Outputs = new Dictionary<string, object>();
        Inputs = new Dictionary<string, object>();

        Map = map;
        _engine = engine;
        Width = map.Width;
        Height = map.Height;
    }

    public void AddOutput(string key, object value)
    {
        Outputs[key] = value;
    }

    public object GetOutput(string key)
    {
        Outputs.TryGetValue(key, out var value);
        return value;
    }


    public JsValue AsInputs()
    {
        return JsValue.FromObject(_engine, Inputs);
    }

    public void ApplyTerrain(int x, int y, TerrainGameObject terrain)
    {
        terrain.Position = new Point(x, y);
        Map.SetTerrain(terrain);
    }

    public void SetLevel(int level)
    {
        Map.Level = level;
    }

    public void SetDescription(string description)
    {
        Map.Description = description;
    }

    public void SetName(string name)
    {
        Map.Name = name;
    }
}
