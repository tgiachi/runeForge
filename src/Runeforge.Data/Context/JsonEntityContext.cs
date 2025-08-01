using System.Text.Json.Serialization;
using Runeforge.Data.Entities.Base;
using Runeforge.Data.Entities.Common;
using Runeforge.Data.Entities.Items;
using Runeforge.Data.Entities.Keysets;
using Runeforge.Data.Entities.Names;
using Runeforge.Data.Entities.Npcs;
using Runeforge.Data.Entities.Tileset;

namespace Runeforge.Data.Context;

[JsonSerializable(typeof(BaseJsonEntityData))]
[JsonSerializable(typeof(BaseJsonEntityData[]))]
[JsonSerializable(typeof(JsonTilesetData))]
[JsonSerializable(typeof(JsonColorData))]
[JsonSerializable(typeof(JsonTileData))]
[JsonSerializable(typeof(JsonTileAnimationData))]
[JsonSerializable(typeof(JsonKeybindData))]
[JsonSerializable(typeof(JsonNameData))]
[JsonSerializable(typeof(JsonItemData))]
[JsonSerializable(typeof(JsonHasTile))]
[JsonSerializable(typeof(JsonItemContainerData))]
[JsonSerializable(typeof(JsonLightSourceData))]
[JsonSerializable(typeof(JsonItemStatData))]
[JsonSerializable(typeof(JsonTileAnimationColor))]
[JsonSerializable(typeof(JsonNpcData))]
[JsonSerializable(typeof(JsonNpcStat))]
[JsonSerializable(typeof(JsonNpcInventory))]
public partial class JsonEntityContext : JsonSerializerContext
{
}
