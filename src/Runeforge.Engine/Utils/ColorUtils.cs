using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Engine.Utils;

public static class ColorUtils
{
    public static ColoredGlyph Darken(ColoredGlyph glyph, float amount)
    {
        var darkenedForeground = new Color(
            (byte)(glyph.Foreground.R * (1 - amount)),
            (byte)(glyph.Foreground.G * (1 - amount)),
            (byte)(glyph.Foreground.B * (1 - amount))
        );

        var darkenedBackground = new Color(
            (byte)(glyph.Background.R * (1 - amount)),
            (byte)(glyph.Background.G * (1 - amount)),
            (byte)(glyph.Background.B * (1 - amount))
        );


        return new ColoredGlyph(darkenedForeground, darkenedBackground, glyph.Glyph, glyph.Mirror);
    }
}
