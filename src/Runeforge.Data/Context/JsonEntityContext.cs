using System.Text.Json.Serialization;
using Runeforge.Data.Entities.Common;
using Runeforge.Data.Entities.Tileset;

namespace Runeforge.Data.Context;

[JsonSerializable(typeof(JsonTilesetData))]
[JsonSerializable(typeof(JsonColorData))]
[JsonSerializable(typeof(JsonTileData))]
[JsonSerializable(typeof(JsonTileAnimationData))]

public partial class JsonEntityContext : JsonSerializerContext
{
}
