using Runeforge.Data.Entities.Tileset;
using Runeforge.Engine.Interfaces.Services.Base;
using Runeforge.Engine.Services;

namespace Runeforge.Engine.Interfaces.Services;

public interface ITileSetService : IRuneforgeService
{
    void AddTile(string tileSet, JsonTileData tileData);
    TileColoredGlyph CreateGlyph(string nameOrTag);

    void SetDefaultTileSet(string tileSetId);

    string GetDefaultTileSet();
}
