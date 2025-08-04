using Runeforge.Engine.GameObjects;
using SadRogue.Primitives;

namespace Runeforge.Engine.Maps.Generators;

/// <summary>
///     Represents a placed building instance with SadConsole integration
/// </summary>
public class PlacedBuilding
{
    public PlacedBuilding(Rectangle bounds, BuildingTemplate template)
    {
        Bounds = bounds;
        Template = template;
        Doors = new List<Point>();
        TerrainObjects = new List<TerrainGameObject>();
    }

    public Rectangle Bounds { get; }
    public BuildingTemplate Template { get; }
    public List<Point> Doors { get; }
    public List<TerrainGameObject> TerrainObjects { get; }

    public void AddDoor(Point door)
    {
        Doors.Add(door);
    }

    public void AddTerrain(TerrainGameObject terrain)
    {
        TerrainObjects.Add(terrain);
    }
}
