namespace Runeforge.Data.Colors;

public readonly struct ColorDef(byte r, byte g, byte b, byte a = 255)
{
    public byte R { get; } = r;
    public byte G { get; } = g;
    public byte B { get; } = b;
    public byte A { get; } = a;

    public override string ToString() => $"ColorDef(R: {R}, G: {G}, B: {B}, A: {A})";
}

