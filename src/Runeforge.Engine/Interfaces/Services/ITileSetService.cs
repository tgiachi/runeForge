using Runeforge.Data.Entities.Tileset;
using Runeforge.Engine.Interfaces.Services.Base;

namespace Runeforge.Engine.Interfaces.Services;

public interface ITileSetService : IRuneforgeService
{
    void AddTile(string tileSet, JsonTileData tileData);
}
