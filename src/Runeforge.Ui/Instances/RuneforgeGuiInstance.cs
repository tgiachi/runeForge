using System.ComponentModel;
using System.Runtime.CompilerServices;
using Runeforge.Engine.Data.Configs.Sections;
using SadConsole;
using SadRogue.Primitives;

namespace Runeforge.Ui.Instances;

public class RuneforgeGuiInstance : INotifyPropertyChanged
{
    private static RuneforgeGuiInstance? _instance;
    public static RuneforgeGuiInstance Instance => _instance ??= new RuneforgeGuiInstance();

    public delegate void FontChangedHandler(IFont font);
    public delegate void FontSizeChangedHandler(Point size);

    public event FontChangedHandler? OnDefaultUiFontChanged;
    public event FontSizeChangedHandler? OnDefaultUiFontSizeChanged;
    public event FontChangedHandler? OnMapFontChanged;
    public event FontSizeChangedHandler? OnMapFontSizeChanged;


    public IFont.Sizes DefaultUiFontSize { get; set; }
    public IFont.Sizes DefaultMapFontSize { get; set; }

    public IFont DefaultUiFont { get; set; }
    public IFont DefaultMapFont { get; set; }

    public GameWindowConfig GameWindowConfig { get; set; }


    public RuneforgeGuiInstance()
    {
        PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DefaultUiFont))
        {
            OnDefaultUiFontChanged?.Invoke(DefaultUiFont);
            return;
        }

        if (e.PropertyName == nameof(DefaultUiFontSize))
        {
            OnDefaultUiFontSizeChanged?.Invoke(DefaultUiFont.GetFontSize(DefaultUiFontSize));
            return;
        }

        if (e.PropertyName == nameof(DefaultMapFont))
        {
            OnMapFontChanged?.Invoke(DefaultMapFont);
            return;
        }

        if (e.PropertyName == nameof(DefaultMapFontSize))
        {
            OnMapFontSizeChanged?.Invoke(DefaultMapFont.GetFontSize(DefaultMapFontSize));
            return;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
