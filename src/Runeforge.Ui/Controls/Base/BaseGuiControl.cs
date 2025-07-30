using System.ComponentModel;
using Runeforge.Ui.Instances;
using SadConsole;
using SadConsole.Input;
using SadConsole.UI;
using SadRogue.Primitives;
using Serilog;

namespace Runeforge.Ui.Controls.Base;

public class BaseGuiControl : ControlsConsole
{
    public bool EscToCloseEnabled { get; set; } = true;
    public bool DrawBorderEnabled { get; set; } = true;
    public bool FocusOnShowEnabled { get; set; }
    public bool CenterOnShowEnabled { get; set; } = false;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string? Title { get; set; }

    private readonly string? _keyBindingName;


    public BaseGuiControl(Point size, string keyBindingName = null, string title = null) : base(size.X, size.Y)
    {
        Font = RuneforgeGuiInstance.Instance.DefaultUiFont;

        RuneforgeGuiInstance.Instance.OnDefaultUiFontChanged += OnDefaultFontChanged;
        _keyBindingName = keyBindingName;

        Title = title;

        if (DrawBorderEnabled)
        {
            DrawBorder();
        }

        if (!string.IsNullOrEmpty(Title))
        {
            DrawTitle();
        }

        ParentChanged += (sender, args) =>
        {
            if (args.OldValue == null && args.NewValue != null)
            {
                OnShown();

                if (CenterOnShowEnabled)
                {
                    ToCenter();
                }

                if (FocusOnShowEnabled)
                {
                    IsFocused = true;
                }

                return;
            }

            if (args.OldValue != null && args.NewValue == null)
            {
                IsFocused = false;
                OnClosed();
            }
        };
    }

    private void OnDefaultFontChanged(IFont font)
    {
        Font = font;
        Resize(Width, Height, true); // Resize to apply the new font size
    }


    protected void ToCenter()
    {
        Position = new Point(
            (RuneforgeGuiInstance.Instance.GameWindowConfig.Width) / 2 - Width / 2,
            (RuneforgeGuiInstance.Instance.GameWindowConfig.Height) / 2 - Height / 2
        );

        IsDirty = true;
    }

    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        if (_keyBindingName != null)
        {
        }

        if (keyboard.IsKeyPressed(Keys.Escape) && EscToCloseEnabled)
        {
            Close();

            return true;
        }

        return base.ProcessKeyboard(keyboard);
    }


    protected void Close()
    {
        RuneforgeGuiInstance.Instance.OnDefaultUiFontChanged -= OnDefaultFontChanged;
        IsEnabled = false;
        IsVisible = false;
        UseKeyboard = false;
        UseMouse = false;
        Parent.Children.Remove(this);
    }

    protected void DrawBorder()
    {
        var horizontal = 196;
        var vertical = 179;    // │ (ASCII 179 o Unicode '\u2502')
        var topLeft = 218;     // ┌ (ASCII 218 o Unicode '\u250C')
        var topRight = 191;    // ┐ (ASCII 191 o Unicode '\u2510')
        var bottomLeft = 192;  // └ (ASCII 192 o Unicode '\u2514')
        var bottomRight = 217; // ┘ (ASCII 217 o Unicode '\u2518')

        this.SetGlyph(0, 0, topLeft);
        this.SetGlyph(Width - 1, 0, topRight);
        this.SetGlyph(0, Height - 1, bottomLeft);
        this.SetGlyph(Width - 1, Height - 1, bottomRight);

        for (int x = 1; x < Width - 1; x++)
        {
            this.SetGlyph(x, 0, horizontal);          // Top border
            this.SetGlyph(x, Height - 1, horizontal); // Bottom border
        }

        for (int y = 1; y < Height - 1; y++)
        {
            this.SetGlyph(0, y, vertical);         // Left border
            this.SetGlyph(Width - 1, y, vertical); // Right border
        }
    }

    private void DrawTitle()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return;
        }

        var titleText = Title;

        if (titleText.Length > Width - 2)
        {
            titleText = titleText[..(Width - 2)];
        }

        var titleX = (Width - titleText.Length) / 2;
        this.Print(titleX, 0, titleText);
    }

    protected virtual void OnShown()
    {
        Log.Logger.Information("Control shown");
    }

    protected virtual void OnClosed()
    {
        Log.Logger.Information("Control closed");
    }

    protected virtual void Draw()
    {
    }

    protected Point FullWithoutBorderSize => new(Width - 2, Height - 2);
}
