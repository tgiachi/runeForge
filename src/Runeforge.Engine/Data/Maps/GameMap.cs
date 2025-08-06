using GoRogue.GameFramework;
using Runeforge.Data.Types.Map;
using SadRogue.Integration.Maps;
using SadRogue.Primitives;

namespace Runeforge.Engine.Data.Maps;

public class GameMap : RogueLikeMap
{
    private readonly Dictionary<MapLayer, List<IGameObject>> _entities = new();

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
    }


    public IEnumerable<TEntity> GetEntitiesFromLayer<TEntity>(MapLayer layerType) where TEntity : IGameObject =>
        _entities[layerType].OfType<TEntity>();
}
