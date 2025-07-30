using Runeforge.Engine.Data.Configs.Sections;
using Runeforge.Engine.Instance;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Ui.Instances;

public class RuneforgeGuiInstance
{
    private static RuneforgeGuiInstance? _instance;
    public static RuneforgeGuiInstance Instance => _instance ??= new RuneforgeGuiInstance();

    public delegate void OnFontChangedHandler(IFont font);
    public delegate void OnSizeChangedHandler(Point size);

    public event OnFontChangedHandler? OnDefaultUiFontChanged;
    public event OnSizeChangedHandler? OnDefaultUiFontSizeChanged;

    public GameWindowConfig GameWindowConfig { get; set; }

    private IFont _font;

    private IFont.Sizes _defaultUiFontSize;

    public IFont.Sizes DefaultUiFontSize
    {
        get => _defaultUiFontSize;
        set
        {
            if (_defaultUiFontSize != value)
            {
                _defaultUiFontSize = value;
                OnDefaultUiFontSizeChanged?.Invoke(DefaultUiFont.GetFontSize(value));
            }
        }
    }
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
