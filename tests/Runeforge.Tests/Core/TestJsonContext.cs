using System.Text.Json.Serialization;

namespace Runeforge.Tests.Core;

/// <summary>
/// JSON context for test models
/// </summary>
[JsonSerializable(typeof(TestModel))]
[JsonSerializable(typeof(TestModelWithEnum))]
[JsonSerializable(typeof(TestEnum))]
[JsonSerializable(typeof(List<TestModel>))]
public partial class TestJsonContext : JsonSerializerContext
{
}
