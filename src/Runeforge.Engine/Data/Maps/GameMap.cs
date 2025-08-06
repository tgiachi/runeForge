using GoRogue.GameFramework;
using Runeforge.Data.Types.Map;
using SadRogue.Integration.Maps;
using SadRogue.Primitives;
using SadRogue.Primitives.SpatialMaps;

namespace Runeforge.Engine.Data.Maps;

public class GameMap : RogueLikeMap
{
    private readonly Dictionary<MapLayer, List<IGameObject>> _entities = new();

    public delegate void EntityHandler(IGameObject gameObject, MapLayer layer);

    public event EntityHandler? EntityAdded;

    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; }
    public string Description { get; set; }
    public int Level { get; set; } = 0;


    public GameMap(int width, int height, DefaultRendererParams? defaultRendererParams)
        : base(width, height, defaultRendererParams, Enum.GetValues<MapLayer>().Length, Distance.Euclidean)
    {
        foreach (var layerType in Enum.GetValues<MapLayer>())
        {
            _entities.Add(layerType, []);
        }

        ObjectAdded += OnObjectAdded;
        ObjectRemoved += OnObjectRemoved;
    }

    private void OnObjectRemoved(object? sender, ItemEventArgs<IGameObject> e)
    {
        if (_entities.TryGetValue((MapLayer)e.Item.Layer, out var entities))
        {
            entities.Remove(e.Item);
        }

        EntityAdded?.Invoke(e.Item, (MapLayer)e.Item.Layer);
    }

    private void OnObjectAdded(object? sender, ItemEventArgs<IGameObject> e)
    {
        EntityAdded?.Invoke(e.Item, (MapLayer)e.Item.Layer);
    }


    public IEnumerable<TEntity> GetEntitiesFromLayer<TEntity>(MapLayer layerType) where TEntity : IGameObject =>
        _entities[layerType].OfType<TEntity>();
}
