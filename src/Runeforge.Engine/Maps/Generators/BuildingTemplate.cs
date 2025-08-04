namespace Runeforge.Engine.Maps.Generators;

/// <summary>
///     Building template that defines the layout of a building
/// </summary>
public class BuildingTemplate
{
    public BuildingTemplate(string[] layoutLines, CityTileType buildingType, string name, string id)
    {
        if (layoutLines == null || layoutLines.Length == 0)
        {
            throw new ArgumentException("Layout cannot be empty");
        }

        Height = layoutLines.Length;
        Width = layoutLines[0].Length;
        Name = name;
        Id = id;
        BuildingType = buildingType;

        if (layoutLines.Any(line => line.Length != Width))
        {
            throw new ArgumentException("All layout lines must have the same width");
        }

        Layout = new char[Width, Height];

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                Layout[x, y] = layoutLines[y][x];
            }
        }
    }

    public char[,] Layout { get; }
    public int Width { get; }
    public int Height { get; }
    public CityTileType BuildingType { get; }
    public string Name { get; }
    public string Id { get; }

    /// <summary>
    ///     Get tile type for a character in the template
    /// </summary>
    public static CityTileType CharToTileType(char c, CityTileType buildingType)
    {
        return c switch
        {
            '#' => CityTileType.Wall,
            '.' => CityTileType.Floor,
            '+' => CityTileType.Door,
            'H' => CityTileType.House,
            'S' => CityTileType.Shop,
            'M' => CityTileType.Market,
            ' ' => CityTileType.Floor,
            _   => buildingType
        };
    }
}
