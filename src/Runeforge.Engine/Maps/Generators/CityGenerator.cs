using Runeforge.Engine.GameObjects;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Engine.Maps.Generators;

/// <summary>
///     City generator integrated with SadConsole/RuneForge
/// </summary>
public class CityGenerator
{
    // Configuration constants
    private const int RoadSpacingVertical = 15;
    private const int RoadSpacingHorizontal = 12;
    private const int MinBuildingsRequired = 6;
    private readonly List<BuildingTemplate> _buildingTemplates;
    private readonly int _height;
    private readonly Random _random;
    private readonly int _width;
    private readonly List<BuildingPlacement> _buildingPlacements;
    private readonly List<PlacedBuilding> _buildings;
    private CityTileType[,] _map;
    private readonly HashSet<Point> _roadTiles;

    public CityGenerator(int width, int height, List<BuildingTemplate> buildingTemplates, int seed = 0)
    {
        _width = width;
        _height = height;
        _random = seed == 0 ? new Random() : new Random(seed);
        _buildingTemplates = buildingTemplates ?? throw new ArgumentNullException(nameof(buildingTemplates));
        _buildings = new List<PlacedBuilding>();
        _buildingPlacements = new List<BuildingPlacement>();
        _roadTiles = new HashSet<Point>();

        if (_buildingTemplates.Count == 0)
        {
            throw new ArgumentException("At least one building template is required");
        }
    }

    /// <summary>
    ///     Public property to access buildings (read-only)
    /// </summary>
    public IReadOnlyList<PlacedBuilding> Buildings => _buildings.AsReadOnly();

    /// <summary>
    ///     Public property to access building placements with positions and IDs
    /// </summary>
    public IReadOnlyList<BuildingPlacement> BuildingPlacements => _buildingPlacements.AsReadOnly();

    /// <summary>
    ///     Get building placement by ID
    /// </summary>
    public BuildingPlacement? GetBuildingById(string buildingId)
    {
        return _buildingPlacements.FirstOrDefault(b => b.BuildingId == buildingId);
    }

    /// <summary>
    ///     Get all buildings of a specific type
    /// </summary>
    public IEnumerable<BuildingPlacement> GetBuildingsByType(CityTileType buildingType)
    {
        return _buildingPlacements.Where(b => b.BuildingType == buildingType);
    }

    /// <summary>
    ///     Get building at specific coordinates
    /// </summary>
    public BuildingPlacement? GetBuildingAtPosition(Point position)
    {
        return _buildingPlacements.FirstOrDefault(b => b.Position.Contains(position));
    }

    /// <summary>
    ///     Generate city and create TerrainGameObjects for SadConsole
    /// </summary>
    public List<TerrainGameObject> GenerateCityTerrain()
    {
        InitializeMap();
        GenerateRoadNetwork();
        GenerateBuildings();
        AddCityPerimeter();
        return CreateTerrainObjects();
    }

    /// <summary>
    ///     Create TerrainGameObject instances from the generated map
    /// </summary>
    private List<TerrainGameObject> CreateTerrainObjects()
    {
        var terrainObjects = new List<TerrainGameObject>();

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                var position = new Point(x, y);
                var tileType = _map[x, y];
                var (appearance, walkable, transparent, tileId) = GetTileProperties(tileType);

                var terrain = new TerrainGameObject(
                    position,
                    appearance,
                    tileId,
                    walkable,
                    transparent
                );

                terrainObjects.Add(terrain);
            }
        }

        return terrainObjects;
    }

    /// <summary>
    ///     Get visual and gameplay properties for each tile type
    /// </summary>
    private (ColoredGlyph appearance, bool walkable, bool transparent, string tileId) GetTileProperties(
        CityTileType tileType
    )
    {
        return tileType switch
        {
            CityTileType.Empty  => (new ColoredGlyph(Color.Green, Color.Black, '.'), true, true, "grass"),
            CityTileType.Road   => (new ColoredGlyph(Color.Gray, Color.DarkGray, '#'), true, true, "road"),
            CityTileType.Wall   => (new ColoredGlyph(Color.White, Color.Black, '█'), false, false, "wall"),
            CityTileType.Floor  => (new ColoredGlyph(Color.Brown, Color.Black, '.'), true, true, "floor"),
            CityTileType.Door   => (new ColoredGlyph(Color.Yellow, Color.Black, '+'), true, true, "door"),
            CityTileType.House  => (new ColoredGlyph(Color.Blue, Color.Black, 'H'), false, true, "house_interior"),
            CityTileType.Shop   => (new ColoredGlyph(Color.Cyan, Color.Black, 'S'), false, true, "shop_interior"),
            CityTileType.Market => (new ColoredGlyph(Color.Magenta, Color.Black, 'M'), false, true, "market_interior"),
            _                   => (new ColoredGlyph(Color.Red, Color.Black, '?'), true, true, "unknown")
        };
    }

    /// <summary>
    ///     Initialize the map with empty tiles
    /// </summary>
    private void InitializeMap()
    {
        _map = new CityTileType[_width, _height];

        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                _map[x, y] = CityTileType.Empty;
            }
        }
    }

    /// <summary>
    ///     Generate a clean road network in a grid pattern
    /// </summary>
    private void GenerateRoadNetwork()
    {
        // Vertical roads
        for (var x = RoadSpacingVertical; x < _width - 1; x += RoadSpacingVertical)
        {
            for (var y = 1; y < _height - 1; y++)
            {
                SetTile(x, y, CityTileType.Road);
                _roadTiles.Add(new Point(x, y));
            }
        }

        // Horizontal roads
        for (var y = RoadSpacingHorizontal; y < _height - 1; y += RoadSpacingHorizontal)
        {
            for (var x = 1; x < _width - 1; x++)
            {
                SetTile(x, y, CityTileType.Road);
                _roadTiles.Add(new Point(x, y));
            }
        }
    }

    /// <summary>
    ///     Generate all buildings using provided templates
    /// </summary>
    private void GenerateBuildings()
    {
        var maxWidth = _buildingTemplates.Max(t => t.Width);
        var maxHeight = _buildingTemplates.Max(t => t.Height);

        // First pass: systematic placement
        for (var y = 2; y < _height - maxHeight - 2; y += 2)
        {
            for (var x = 2; x < _width - maxWidth - 2; x += 2)
            {
                if (CanPlaceBuildingAt(x, y))
                {
                    var building = TryCreateBuildingAt(x, y);
                    if (building != null)
                    {
                        PlaceBuilding(building);
                        _buildings.Add(building);
                    }
                }
            }
        }

        // Ensure minimum building count
        if (_buildings.Count < MinBuildingsRequired)
        {
            EnsureMinimumBuildings(maxWidth, maxHeight);
        }
    }

    /// <summary>
    ///     Try to create a building at the specified position using available templates
    /// </summary>
    private PlacedBuilding? TryCreateBuildingAt(int startX, int startY)
    {
        var shuffledTemplates = _buildingTemplates.OrderBy(x => _random.Next()).ToList();

        foreach (var template in shuffledTemplates)
        {
            var bounds = new Rectangle(startX, startY, template.Width, template.Height);

            if (IsAreaClear(bounds) && IsNearRoad(startX, startY, 3))
            {
                return new PlacedBuilding(bounds, template);
            }
        }

        return null;
    }

    /// <summary>
    ///     Check if a building can be placed at the given position
    /// </summary>
    private bool CanPlaceBuildingAt(int x, int y)
    {
        if (GetTile(x, y) != CityTileType.Empty)
        {
            return false;
        }

        return IsNearRoad(x, y, 3) && !IsOnRoad(x, y);
    }

    /// <summary>
    ///     Check if an area is completely clear for building
    /// </summary>
    private bool IsAreaClear(Rectangle area)
    {
        if (area.MaxExtentX >= _width - 1 || area.MaxExtentY >= _height - 1)
        {
            return false;
        }

        for (var x = area.X; x <= area.MaxExtentX; x++)
        {
            for (var y = area.Y; y <= area.MaxExtentY; y++)
            {
                if (GetTile(x, y) != CityTileType.Empty)
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    ///     Place a building on the map using its template
    /// </summary>
    private void PlaceBuilding(PlacedBuilding building)
    {
        var template = building.Template;
        var bounds = building.Bounds;

        var placement = new BuildingPlacement(
            template.Id,
            template.Name,
            bounds,
            template.BuildingType
        );

        for (var x = 0; x < template.Width; x++)
        {
            for (var y = 0; y < template.Height; y++)
            {
                var templateChar = template.Layout[x, y];
                var tileType = BuildingTemplate.CharToTileType(templateChar, template.BuildingType);

                var mapX = bounds.X + x;
                var mapY = bounds.Y + y;

                SetTile(mapX, mapY, tileType);

                if (tileType == CityTileType.Door)
                {
                    var doorPos = new Point(mapX, mapY);
                    building.AddDoor(doorPos);
                    placement.AddDoor(doorPos);
                }
            }
        }

        _buildingPlacements.Add(placement);
    }

    /// <summary>
    ///     Ensure we have at least the minimum required number of buildings
    /// </summary>
    private void EnsureMinimumBuildings(int maxWidth, int maxHeight)
    {
        var attempts = 0;
        var maxAttempts = 1000;

        while (_buildings.Count < MinBuildingsRequired && attempts < maxAttempts)
        {
            var x = _random.Next(2, _width - maxWidth - 2);
            var y = _random.Next(2, _height - maxHeight - 2);

            if (CanPlaceBuildingAt(x, y))
            {
                var building = TryCreateBuildingAt(x, y);
                if (building != null)
                {
                    PlaceBuilding(building);
                    _buildings.Add(building);
                }
            }

            attempts++;
        }
    }

    /// <summary>
    ///     Add city perimeter walls with gates
    /// </summary>
    private void AddCityPerimeter()
    {
        for (var x = 0; x < _width; x++)
        {
            SetTile(x, 0, CityTileType.Wall);
            SetTile(x, _height - 1, CityTileType.Wall);
        }

        for (var y = 0; y < _height; y++)
        {
            SetTile(0, y, CityTileType.Wall);
            SetTile(_width - 1, y, CityTileType.Wall);
        }

        // Add gates where roads meet perimeter
        foreach (var roadTile in _roadTiles)
        {
            if (roadTile.Y == 1)
            {
                SetTile(roadTile.X, 0, CityTileType.Door);
            }

            if (roadTile.Y == _height - 2)
            {
                SetTile(roadTile.X, _height - 1, CityTileType.Door);
            }

            if (roadTile.X == 1)
            {
                SetTile(0, roadTile.Y, CityTileType.Door);
            }

            if (roadTile.X == _width - 2)
            {
                SetTile(_width - 1, roadTile.Y, CityTileType.Door);
            }
        }
    }

    /// <summary>
    ///     Utility methods for map access and checks
    /// </summary>
    private void SetTile(int x, int y, CityTileType tile)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _map[x, y] = tile;
        }
    }

    private CityTileType GetTile(int x, int y)
    {
        return x >= 0 && x < _width && y >= 0 && y < _height ? _map[x, y] : CityTileType.Wall;
    }

    private bool IsNearRoad(int x, int y, int maxDistance)
    {
        for (var dx = -maxDistance; dx <= maxDistance; dx++)
        {
            for (var dy = -maxDistance; dy <= maxDistance; dy++)
            {
                if (_roadTiles.Contains(new Point(x + dx, y + dy)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsOnRoad(int x, int y)
    {
        return _roadTiles.Contains(new Point(x, y));
    }
}
