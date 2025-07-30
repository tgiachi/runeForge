using System.Collections.Concurrent;
using Runeforge.Engine.Logger.Sink;
using Runeforge.Ui.Screens.Base;
using SadConsole;
using SadConsole.Input;
using SadRogue.Primitives;
using Serilog.Events;

namespace Runeforge.UI.Screens;

/// <summary>
///     SadConsole screen for displaying log entries with scrolling and colors
/// </summary>
public class LogViewerScreen : BaseRuneforgeScreenSurface
{
    private readonly List<LogEntry> _displayedLogs = new();
    private readonly Lock _lockObject = new();

    /// <summary>
    ///     Color scheme for different log levels
    /// </summary>
    private readonly Dictionary<LogEventLevel, Color> _logColors = new()
    {
        { LogEventLevel.Verbose, Color.Gray },
        { LogEventLevel.Debug, Color.Cyan },
        { LogEventLevel.Information, Color.White },
        { LogEventLevel.Warning, Color.Yellow },
        { LogEventLevel.Error, Color.Red },
        { LogEventLevel.Fatal, Color.DarkRed }
    };

    private readonly ConcurrentQueue<LogEntry> _logEntries = new();
    private int _maxLogEntries = 1000;

    private int _scrollOffset;
    private int _visibleLines;

    public LogViewerScreen(int width, int height) : base(width, height)
    {
        _visibleLines = height - 2; // Reserve space for border/title

        // Set up the surface
        Surface.DefaultBackground = Color.Black;
        Surface.DefaultForeground = Color.White;
        Surface.Clear();

        // Draw border and title
        DrawBorder();
        DrawTitle();

        // Enable keyboard input for scrolling
        UseKeyboard = true;
        IsFocused = true;
    }

    /// <summary>
    ///     Add a log entry to the viewer
    /// </summary>
    public void AddLogEntry(LogEntry logEntry)
    {
        _logEntries.Enqueue(logEntry);

        // Process pending log entries
        ProcessPendingLogs();
    }

    /// <summary>
    ///     Process all pending log entries from the queue
    /// </summary>
    private void ProcessPendingLogs()
    {
        lock (_lockObject)
        {
            while (_logEntries.TryDequeue(out var logEntry))
            {
                _displayedLogs.Add(logEntry);

                // Limit the number of stored log entries
                if (_displayedLogs.Count > _maxLogEntries)
                {
                    _displayedLogs.RemoveAt(0);

                    // Adjust scroll offset if we removed entries from the beginning
                    if (_scrollOffset > 0)
                    {
                        _scrollOffset--;
                    }
                }
            }

            // Auto-scroll to bottom when new logs arrive (if not manually scrolled)
            if (_scrollOffset == 0)
            {
                AutoScrollToBottom();
            }

            RefreshDisplay();
        }
    }

    /// <summary>
    ///     Auto-scroll to show the latest log entries
    /// </summary>
    private void AutoScrollToBottom()
    {
        var totalLines = _displayedLogs.Count;
        if (totalLines > _visibleLines)
        {
            _scrollOffset = 0; // 0 means showing the bottom
        }
    }

    /// <summary>
    ///     Refresh the display with current log entries
    /// </summary>
    private void RefreshDisplay()
    {
        // Clear the log area (preserve border)
        ClearLogArea();

        var totalLines = _displayedLogs.Count;
        var startIndex = Math.Max(0, totalLines - _visibleLines - _scrollOffset);
        var endIndex = Math.Min(totalLines, startIndex + _visibleLines);

        var displayLine = 1; // Start after the title line

        for (var i = startIndex; i < endIndex; i++)
        {
            var logEntry = _displayedLogs[i];
            DrawLogEntry(logEntry, displayLine);
            displayLine++;
        }

        // Draw scroll indicator
        DrawScrollIndicator();

        IsDirty = true;
    }

    /// <summary>
    ///     Draw a single log entry on the specified line
    /// </summary>
    private void DrawLogEntry(LogEntry logEntry, int line)
    {
        if (line >= Height - 1)
        {
            return; // Don't draw outside bounds
        }

        var color = _logColors.GetValueOrDefault(logEntry.Level, Color.White);
        var timestamp = logEntry.Timestamp.ToString("HH:mm:ss");
        var levelText = GetLevelText(logEntry.Level);

        // Format: [HH:mm:ss] [LEVEL] Category: Message
        var prefix = $"[{timestamp}] [{levelText}]";
        var categoryText = !string.IsNullOrEmpty(logEntry.Category) ? $" {logEntry.Category}:" : "";
        var message = logEntry.Message;

        var fullText = $"{prefix}{categoryText} {message}";

        // Truncate if too long
        var maxWidth = Width - 2; // Account for border
        if (fullText.Length > maxWidth)
        {
            fullText = fullText.Substring(0, maxWidth - 3) + "...";
        }

        // Clear the line first
        Surface.Print(1, line, new string(' ', maxWidth), Color.White, Color.Black);

        // Print timestamp and level with color
        var x = 1;
        Surface.Print(x, line, prefix, color, Color.Black);
        x += prefix.Length;

        // Print category in gray
        if (!string.IsNullOrEmpty(categoryText))
        {
            Surface.Print(x, line, categoryText, Color.Gray, Color.Black);
            x += categoryText.Length;
        }

        // Print message
        var remainingSpace = maxWidth - (x - 1);
        if (message.Length > remainingSpace)
        {
            message = message.Substring(0, remainingSpace - 3) + "...";
        }

        Surface.Print(x, line, $" {message}", Color.White, Color.Black);
    }

    /// <summary>
    ///     Get short text representation of log level
    /// </summary>
    private static string GetLevelText(LogEventLevel level)
    {
        return level switch
        {
            LogEventLevel.Verbose     => "VRB",
            LogEventLevel.Debug       => "DBG",
            LogEventLevel.Information => "INF",
            LogEventLevel.Warning     => "WRN",
            LogEventLevel.Error       => "ERR",
            LogEventLevel.Fatal       => "FTL",
            _                         => "UNK"
        };
    }

    /// <summary>
    ///     Clear the log display area (preserve borders)
    /// </summary>
    private void ClearLogArea()
    {
        for (var y = 1; y < Height - 1; y++)
        {
            for (var x = 1; x < Width - 1; x++)
            {
                Surface.SetGlyph(x, y, ' ', Color.White, Color.Black);
            }
        }
    }

    /// <summary>
    ///     Draw the border around the log viewer
    /// </summary>
    private void DrawBorder()
    {
        Surface.DrawBox(
            new Rectangle(0, 0, Width, Height),
            ShapeParameters.CreateStyledBox(
                ICellSurface.ConnectedLineThin,
                new ColoredGlyph(Color.Gray, Color.Black)
            )
        );
    }

    /// <summary>
    ///     Draw the title bar
    /// </summary>
    private void DrawTitle()
    {
        var title = " Runeforge Log Viewer ";
        var x = (Width - title.Length) / 2;
        Surface.Print(x, 0, title, Color.White, Color.Black);
    }

    /// <summary>
    ///     Draw scroll indicator showing current position
    /// </summary>
    private void DrawScrollIndicator()
    {
        if (_displayedLogs.Count <= _visibleLines)
        {
            return;
        }

        var totalLines = _displayedLogs.Count;
        var scrollPercent = _scrollOffset == 0
            ? 100
            : (int)((double)(totalLines - _visibleLines - _scrollOffset) / (totalLines - _visibleLines) * 100);

        var indicator = $" {scrollPercent}% ";
        Surface.Print(Width - indicator.Length - 1, 0, indicator, Color.Yellow, Color.Black);
    }

    /// <summary>
    ///     Handle keyboard input for scrolling
    /// </summary>
    public override bool ProcessKeyboard(Keyboard keyboard)
    {
        var wasHandled = false;

        if (keyboard.IsKeyPressed(Keys.Up))
        {
            ScrollUp();
            wasHandled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.Down))
        {
            ScrollDown();
            wasHandled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.PageUp))
        {
            ScrollUp(_visibleLines / 2);
            wasHandled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.PageDown))
        {
            ScrollDown(_visibleLines / 2);
            wasHandled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.Home))
        {
            ScrollToTop();
            wasHandled = true;
        }
        else if (keyboard.IsKeyPressed(Keys.End))
        {
            ScrollToBottom();
            wasHandled = true;
        }

        return wasHandled || base.ProcessKeyboard(keyboard);
    }

    /// <summary>
    ///     Scroll up by specified number of lines
    /// </summary>
    private void ScrollUp(int lines = 1)
    {
        var maxScroll = Math.Max(0, _displayedLogs.Count - _visibleLines);
        _scrollOffset = Math.Min(_scrollOffset + lines, maxScroll);
        RefreshDisplay();
    }

    /// <summary>
    ///     Scroll down by specified number of lines
    /// </summary>
    private void ScrollDown(int lines = 1)
    {
        _scrollOffset = Math.Max(0, _scrollOffset - lines);
        RefreshDisplay();
    }

    /// <summary>
    ///     Scroll to the top of the log
    /// </summary>
    private void ScrollToTop()
    {
        _scrollOffset = Math.Max(0, _displayedLogs.Count - _visibleLines);
        RefreshDisplay();
    }

    /// <summary>
    ///     Scroll to the bottom of the log (latest entries)
    /// </summary>
    private void ScrollToBottom()
    {
        _scrollOffset = 0;
        RefreshDisplay();
    }

    /// <summary>
    ///     Clear all log entries
    /// </summary>
    public void ClearLogs()
    {
        lock (_lockObject)
        {
            _displayedLogs.Clear();
            _scrollOffset = 0;
            RefreshDisplay();
        }
    }

    /// <summary>
    ///     Set maximum number of log entries to keep in memory
    /// </summary>
    public void SetMaxLogEntries(int maxEntries)
    {
        _maxLogEntries = Math.Max(100, maxEntries);
    }
}
