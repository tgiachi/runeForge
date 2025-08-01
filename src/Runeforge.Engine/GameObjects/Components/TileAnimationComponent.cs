using Runeforge.Engine.Services;
using SadConsole;
using SadRogue.Integration;
using SadRogue.Integration.Components;
using SadRogue.Primitives;

namespace Runeforge.Engine.GameObjects.Components;

public class TileAnimationComponent : RogueLikeComponentBase<RogueLikeEntity>
{
    private readonly string[] _frames;

    private int _currentFrameIndex = 0;

    private readonly int _interval;

    private TimeSpan _elapsedTime;

    private Color? _startForeground;
    private Color? _endForeground;
    private Color? _startBackground;
    private Color? _endBackground;

    private bool _isForward = true;


    private TimeSpan _fadeDuration = TimeSpan.FromSeconds(1);

    private int _currentTime = 0;

    public TileAnimationComponent(AnimationData animationData) : base(true, false, false, false)
    {
        _frames = animationData.Frames;
        _interval = animationData.Interval;
    }

    public TileAnimationComponent(
        string[] frames, int interval, Color? startForeground = null, Color? endForeground = null,
        Color? startBackground = null, Color? endBackground = null
    ) : base(true, false, false, false)
    {
        _frames = frames;
        _interval = interval;
        _startForeground = startForeground;
        _endForeground = endForeground;
        _startBackground = startBackground;
        _endBackground = endBackground;
    }


    private void UpdateColors()
    {
        if (!_startForeground.HasValue && !_endForeground.HasValue && !_startBackground.HasValue && !_endBackground.HasValue)
        {
            return;
        }


        double progress = _elapsedTime.TotalMilliseconds / _fadeDuration.TotalMilliseconds;

        progress = Math.Clamp(progress, 0.0, 1.0);

        if (progress >= 1.0)
        {
            _elapsedTime = TimeSpan.Zero;
            _isForward = !_isForward;
            progress = 1.0;
        }
        else if (progress < 0.0)
        {
            progress = 0.0;
        }

        if (!_isForward)
        {
            progress = 1.0 - progress;
        }


        if (_startForeground.HasValue && _endForeground.HasValue)
        {
            Parent.AppearanceSingle.Appearance.Foreground = LerpColor(
                _startForeground.Value,
                _endForeground.Value,
                progress
            );
        }

        if (_startBackground.HasValue && _endBackground.HasValue)
        {
            Parent.AppearanceSingle.Appearance.Background = LerpColor(
                _startBackground.Value,
                _endBackground.Value,
                progress
            );
        }
    }

    public override void Update(IScreenObject host, TimeSpan delta)
    {
        _currentTime += delta.Milliseconds;
        _elapsedTime += delta;

        if (_currentTime >= _interval)
        {
            var glyph = _frames[_currentFrameIndex];
            Parent.AppearanceSingle.Appearance.GlyphCharacter = glyph[0];

            _currentFrameIndex++;

            if (_currentFrameIndex >= _frames.Length)
            {
                _currentFrameIndex = 0;
            }

            _currentTime = 0;
        }

        UpdateColors();


        base.Update(host, delta);
    }

    private static Color LerpColor(Color start, Color end, double progress)
    {
        byte r = (byte)(start.R + (end.R - start.R) * progress);
        byte g = (byte)(start.G + (end.G - start.G) * progress);
        byte b = (byte)(start.B + (end.B - start.B) * progress);

        return new Color(r, g, b);
    }
}
