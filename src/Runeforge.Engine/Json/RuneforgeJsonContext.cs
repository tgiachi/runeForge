using System.Text.Json.Serialization;
using Runeforge.Engine.Data.Configs;

namespace Runeforge.Engine.Json;

[JsonSerializable(typeof(RuneforgeEngineConfig))]
public partial class RuneforgeJsonContext : JsonSerializerContext
{
}
