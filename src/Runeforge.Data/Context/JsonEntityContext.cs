using System.Text.Json.Serialization;

namespace Runeforge.Data.Context;

[JsonSerializable(typeof(Entities.JsonTileData))]
public partial class JsonEntityContext : JsonSerializerContext
{
}
