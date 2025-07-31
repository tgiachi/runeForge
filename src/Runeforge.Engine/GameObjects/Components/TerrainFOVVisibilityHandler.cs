using SadRogue.Integration.FieldOfView.Memory;

namespace Runeforge.Engine.GameObjects.Components;

public class TerrainFOVVisibilityHandler : MemoryFieldOfViewHandlerBase
{
    protected override void ApplyMemoryAppearance(MemoryAwareRogueLikeCell terrain)
    {
        terrain.LastSeenAppearance.CopyAppearanceFrom(((TerrainGameObject)terrain).DarkAppearance);
    }
}
