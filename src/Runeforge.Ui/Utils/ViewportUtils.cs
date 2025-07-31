
using SadRogue.Primitives;

namespace Runeforge.Ui.Utils;

public static class ViewportUtils
{
    public static Point CalculateViewport(
        Point originalViewport, int originalFontWidth, int originalFontHeight, int newFontWidth, int newFontHeight
    )
    {
        var widthRatio = (float)newFontWidth / originalFontWidth;
        var heightRatio = (float)newFontHeight / originalFontHeight;

        var newViewportWidth = (int)(originalViewport.X / widthRatio);
        var newViewportHeight = (int)(originalViewport.Y / heightRatio);


        return new Point(newViewportWidth, newViewportHeight);
    }
}
