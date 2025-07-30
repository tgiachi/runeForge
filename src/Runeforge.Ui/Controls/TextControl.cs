using Runeforge.Ui.Controls.Base;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Ui.Controls;

public class TextControl : BaseGuiControl
{
    public string Text { get; set; }

    public Color Foreground { get; set; } = Color.White;

    public Color Background { get; set; } = Color.Transparent;


    public TextControl(Point size) : base(size)
    {
        PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(Text))
            {
                Draw();
            }

            if (args.PropertyName == nameof(Foreground))
            {
                Draw();
            }

            if (args.PropertyName == nameof(Background))
            {
                Draw();
            }

            if (args.PropertyName == nameof(FontSize))
            {
                Draw();
            }
        };


        Draw();
    }

    protected override void Draw()
    {
        this.Clear();


        this.Print(0, 0, Text, Foreground, Background);
    }
}
