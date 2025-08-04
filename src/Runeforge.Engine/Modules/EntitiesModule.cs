using Runeforge.Engine.Attributes.Scripts;
using Runeforge.Engine.GameObjects;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Engine.Modules;

[ScriptModule("entities")]
public class EntitiesModule
{

    [ScriptFunction("Create entity terrain")]
    public TerrainGameObject CreateTerrain(
        int x, int y, ColoredGlyph coloredGlyph, string tileId, bool isWalkable, bool isTransparent
    )
    {
        return new TerrainGameObject(new Point(x, y), coloredGlyph, tileId, isWalkable, isTransparent);
    }
}
