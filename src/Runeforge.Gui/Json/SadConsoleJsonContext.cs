using System.Text.Json.Serialization;

namespace Runeforge.Gui.Json;

[JsonSerializable(typeof(SadFont))]
[JsonSerializable(typeof(SadFont[]))]
public partial class SadConsoleJsonContext : JsonSerializerContext
{

}
