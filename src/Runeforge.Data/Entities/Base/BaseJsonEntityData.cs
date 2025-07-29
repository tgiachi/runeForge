using System.Text.Json.Serialization;
using Runeforge.Data.Entities.Common;
using Runeforge.Data.Entities.Keysets;
using Runeforge.Data.Entities.Tileset;
using Runeforge.Data.Interfaces;

namespace Runeforge.Data.Entities.Base;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(JsonTilesetData), "tiles")]
[JsonDerivedType(typeof(JsonColorData), "colors")]
[JsonDerivedType(typeof(JsonKeysetData), "keybinds")]
public class BaseJsonEntityData : IJsonEntityData
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<string> Tags { get; set; } = new();
    public string? Description { get; set; }
}
