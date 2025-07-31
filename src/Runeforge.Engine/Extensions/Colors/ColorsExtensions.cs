using Runeforge.Data.Colors;
using SadRogue.Primitives;

namespace Runeforge.Engine.Extensions.Colors;

public static class ColorsExtensions
{
    public static Color ToColor(this ColorDef colorDef)
    {
        return new Color(
            colorDef.R,
            colorDef.G,
            colorDef.B,
            colorDef.A
        );
    }
}
