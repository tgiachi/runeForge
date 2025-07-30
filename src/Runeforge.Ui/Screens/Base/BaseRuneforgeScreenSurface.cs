using Runeforge.Ui.Instances;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Ui.Screens.Base;

/// <summary>
/// Base screen surface that automatically handles font changes
/// </summary>
public abstract class BaseRuneforgeScreenSurface : ScreenSurface
{
    protected BaseRuneforgeScreenSurface(int width, int height) : base(width, height)
    {
        // Subscribe to font changes
        RuneforgeGuiInstance.Instance.OnDefaultUiFontChanged += OnFontChanged;
        RuneforgeGuiInstance.Instance.OnDefaultUiFontSizeChanged += OnFontSizeChanged;

        // Set initial font if available
        if (RuneforgeGuiInstance.Instance.DefaultUiFont != null)
        {
            Font = RuneforgeGuiInstance.Instance.DefaultUiFont;
        }
    }

    private void OnFontSizeChanged(Point size)
    {
        FontSize = new Point(size.X, size.Y);
        IsDirty = true;

        // Update child surfaces if any
        UpdateChildSurfacesFont(Font);

        // Call custom font change logic
        OnFontChangedCustom(Font);
    }

    /// <summary>
    /// Called when the default UI font changes
    /// </summary>
    /// <param name="newFont">The new default font</param>
    protected virtual void OnFontChanged(IFont newFont)
    {
        Font = newFont;
        IsDirty = true;

        // Update child surfaces if any
        UpdateChildSurfacesFont(newFont);

        // Call custom font change logic
        OnFontChangedCustom(newFont);
    }

    /// <summary>
    /// Override this for custom font change logic
    /// </summary>
    /// <param name="newFont">The new font</param>
    protected virtual void OnFontChangedCustom(IFont newFont)
    {
        // Override in derived classes
    }

    /// <summary>
    /// Updates font for child surfaces
    /// </summary>
    private void UpdateChildSurfacesFont(IFont newFont)
    {
        foreach (var child in Children)
        {
            if (child is ScreenSurface surface)
            {
                surface.Font = newFont;
                surface.IsDirty = true;
            }

            if (child is BaseRuneforgeScreenSurface baseChild)
            {
                baseChild.OnFontChanged(newFont);
            }
        }
    }

    /// <summary>
    /// Cleanup event subscription
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            RuneforgeGuiInstance.Instance.OnDefaultUiFontChanged -= OnFontChanged;
        }

        base.Dispose(disposing);
    }
}
