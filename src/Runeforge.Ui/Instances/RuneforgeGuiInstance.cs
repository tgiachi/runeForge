using Runeforge.Engine.Data.Configs.Sections;
using Runeforge.Engine.Instance;
using SadConsole;

namespace Runeforge.Ui.Instances;

public class RuneforgeGuiInstance
{
    private static RuneforgeGuiInstance? _instance;
    public static RuneforgeGuiInstance Instance => _instance ??= new RuneforgeGuiInstance();

    public delegate void OnFontChangedHandler(IFont font);

    public event OnFontChangedHandler? OnDefaultUiFontChanged;

    public GameWindowConfig GameWindowConfig { get; set; }

    private IFont _font;

    public IFont DefaultUiFont
    {
        get => _font;
        set
        {
            if (_font != value)
            {
                _font = value;
                OnDefaultUiFontChanged?.Invoke(_font);
            }
        }
    }
}
