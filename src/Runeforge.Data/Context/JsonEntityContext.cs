using System.Text.Json.Serialization;
using Runeforge.Data.Entities.Common;
using Runeforge.Data.Entities.Keysets;
using Runeforge.Data.Entities.Tileset;

namespace Runeforge.Data.Context;

[JsonSerializable(typeof(JsonTilesetData))]
[JsonSerializable(typeof(JsonColorData))]
[JsonSerializable(typeof(JsonTileData))]
[JsonSerializable(typeof(JsonTileAnimationData))]
[JsonSerializable(typeof(JsonKeybindData))]

public partial class JsonEntityContext : JsonSerializerContext
{
}
