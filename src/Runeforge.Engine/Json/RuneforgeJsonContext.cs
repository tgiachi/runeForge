using System.Text.Json.Serialization;
using Runeforge.Engine.Data.Config;

namespace Runeforge.Engine.Json;

[JsonSerializable(typeof(RuneforgeEngineConfig))]
public partial class RuneforgeJsonContext : JsonSerializerContext
{
}
