namespace Runeforge.Engine.Data.Internal.Services;

public record ServiceDefObject(Type ServiceType, Type ImplementationType, int Priority = 0);
