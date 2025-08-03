using Runeforge.Data.Entities.Tileset;
using Runeforge.Engine.Interfaces.Services.Base;
using Runeforge.Engine.Services;

namespace Runeforge.Engine.Interfaces.Services;

public interface ITileSetService : IRuneforgeService
{
    void AddTile(string tileSet, JsonTileData tileData);

    void AddAnimation(string tileSet, JsonTileAnimationData animationData);

    TileColoredGlyph CreateGlyph(string nameOrTag);

    TileColoredGlyph CreateGlyph(JsonHasTile tileData);

    void SetDefaultTileSet(string tileSetId);

    string GetDefaultTileSet();
}
