namespace Runeforge.Engine.Data.Internal.Scripts;

public record ScriptFunctionParameterDescriptor(
    string ParameterName,
    string ParameterType,
    Type RawParameterType,
    string ParameterTypeString
);
