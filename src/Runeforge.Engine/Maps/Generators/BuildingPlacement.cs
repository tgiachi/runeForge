using SadRogue.Primitives;

namespace Runeforge.Engine.Maps.Generators;

/// <summary>
///     Information about a placed building's location and template
/// </summary>
public class BuildingPlacement
{
    public BuildingPlacement(string buildingId, string templateName, Rectangle position, CityTileType buildingType)
    {
        BuildingId = buildingId;
        TemplateName = templateName;
        Position = position;
        BuildingType = buildingType;
        Doors = new List<Point>();
    }

    public string BuildingId { get; }
    public string TemplateName { get; }
    public Rectangle Position { get; }
    public CityTileType BuildingType { get; }
    public List<Point> Doors { get; }

    public void AddDoor(Point door)
    {
        Doors.Add(door);
    }
}
