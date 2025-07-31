using GoRogue.Components.ParentAware;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;

namespace Runeforge.Engine.GameObjects.Components;

public class PlayerFOVController : RogueLikeComponentBase<RogueLikeEntity>
{
    /// <summary>
    /// The sight radius of the player.
    /// </summary>
    public int FOVRadius { get; set; } = 8;

    public PlayerFOVController(int fovRadius = 8)
        : base(false, false, false, false)
    {
        // When the component is attached/detached from an object, set/reset hooks so that FOV is recalculated when
        // the object moves
        Added += OnAdded;
        Removed += OnRemoved;
        FOVRadius = fovRadius;
    }

    /// <summary>
    /// Calculate player FOV if the parent is part of a map.
    /// </summary>
    public void CalculateFOV()
    {
        Parent?.CurrentMap?.PlayerFOV.Calculate(Parent.Position, FOVRadius, Parent.CurrentMap.DistanceMeasurement);
    }

    private void OnAdded(object? s, EventArgs e)
    {
        Parent!.PositionChanged += OnPositionChanged;
    }

    private void OnRemoved(object? s, ParentAwareComponentRemovedEventArgs<RogueLikeEntity> e)
        => e.OldParent.PositionChanged -= OnPositionChanged;

    private void OnPositionChanged(object? sender, ValueChangedEventArgs<Point> e)
        => CalculateFOV();
}
