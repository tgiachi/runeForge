using Runeforge.Ui.Data.Input;
using Runeforge.Ui.Interfaces.Services;
using SadConsole.Input;
using Serilog;

namespace Runeforge.Ui.Services;

public class InputSystemService : IInputSystemService
{
    public string Context { get; set; } = "default";

    public void AddKeyBinding(string context, string key, string actionName)
    {
        if (string.IsNullOrEmpty(context))
        {
            context = "default";
        }


        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        }

        if (string.IsNullOrEmpty(actionName))
        {
            throw new ArgumentException("Action name cannot be null or empty", nameof(actionName));
        }

        var keyCombination = ParseKeyCombination(key, actionName);
        if (!_contextBindings.TryGetValue(context, out var _))
        {
            _contextBindings[context] = [];
        }

        _contextBindings[context].Add(keyCombination);
    }


    /// <summary>
    /// Stores registered key combinations for each context
    /// Context -> Set of valid KeyCombinations
    /// </summary>
    private readonly Dictionary<string, HashSet<KeyCombination>> _contextBindings = new();

    private readonly ILogger _logger = Log.ForContext<InputSystemService>();


    /// <summary>
    /// Parse a string like "CTRL+ALT+A" into a KeyCombination struct
    /// </summary>
    private static KeyCombination ParseKeyCombination(string keyCombination, string actionName)
    {
        // Clean up the input: remove spaces and normalize case
        var cleanInput = keyCombination.Replace(" ", "").ToUpperInvariant();

        // Split by + to get individual parts
        var parts = cleanInput.Split('+', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
        {
            throw new ArgumentException($"Invalid key combination: '{keyCombination}'");
        }

        var modifiers = new HashSet<Keys>();
        Keys mainKey = Keys.None;

        // Process each part
        foreach (var part in parts)
        {
            var key = ParseSingleKey(part);

            // Check if this is a modifier key
            if (IsModifierKey(key))
            {
                modifiers.Add(key);
            }
            else
            {
                // This should be the main key
                if (mainKey != Keys.None)
                {
                    throw new ArgumentException(
                        $"Multiple non-modifier keys found in '{keyCombination}'. Only one main key allowed."
                    );
                }

                mainKey = key;
            }
        }

        // Must have at least one main key
        if (mainKey == Keys.None)
        {
            throw new ArgumentException(
                $"No main key found in '{keyCombination}'. Must have at least one non-modifier key."
            );
        }

        return new KeyCombination(mainKey, actionName, modifiers);
    }


    /// <summary>
    /// Get the action name for the current input in the current context
    /// This is the main function to check what action corresponds to the pressed keys
    /// </summary>
    /// <param name="pressedKey">The main key that was pressed</param>
    /// <param name="modifiers">Set of modifier keys currently held down</param>
    /// <returns>The action name if found, null if no action is mapped to this key combination</returns>
    public string? GetCurrentAction(Keys pressedKey, HashSet<Keys>? modifiers = null)
    {
        return GetAction(Context, pressedKey, modifiers);
    }

    /// <summary>
    /// Get the action name for input in a specific context
    /// </summary>
    /// <param name="context">The context to check in</param>
    /// <param name="pressedKey">The main key that was pressed</param>
    /// <param name="modifiers">Set of modifier keys currently held down</param>
    /// <returns>The action name if found, null if no action is mapped to this key combination</returns>
    public string? GetAction(string context, Keys pressedKey, HashSet<Keys>? modifiers = null)
    {
        if (string.IsNullOrWhiteSpace(context))
        {
            _logger.Warning("Cannot get action: context is null or empty");
            return null;
        }

        // Check if the context exists
        if (!_contextBindings.TryGetValue(context, out var contextActions))
        {
            _logger.Debug("Context '{Context}' has no registered key combinations", context);
            return null;
        }

        // Create a KeyCombination from the current input
        var currentCombination = new KeyCombination(pressedKey, null, modifiers ?? []);

        // Look up the action for this combination
        if (contextActions.TryGetValue(currentCombination, out var actionName))
        {
            _logger.Debug(
                "Key combination '{KeyCombination}' mapped to action '{ActionName}' in context '{Context}'",
                currentCombination.ToString(),
                actionName.Action,
                context
            );
            return actionName.Action;
        }

        _logger.Debug(
            "Key combination '{KeyCombination}' not mapped to any action in context '{Context}'",
            currentCombination.ToString(),
            context
        );
        return null;
    }

    /// <summary>
    /// Get the action name for input from SadConsole keyboard state
    /// This is the main integration point with SadConsole for getting actions
    /// </summary>
    /// <param name="keyboard">SadConsole keyboard state</param>
    /// <param name="pressedKey">The key that was just pressed</param>
    /// <returns>The action name if found, null if no action is mapped to this key combination</returns>
    public string? GetAction(Keyboard keyboard, Keys pressedKey)
    {
        // Extract currently held modifiers from SadConsole keyboard state
        var modifiers = new HashSet<Keys>();

        if (keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl))
        {
            modifiers.Add(Keys.LeftControl);
        }

        if (keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt))
        {
            modifiers.Add(Keys.LeftAlt);
        }

        if (keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift))
        {
            modifiers.Add(Keys.LeftShift);
        }

        if (keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows))
        {
            modifiers.Add(Keys.LeftWindows);
        }

        return GetCurrentAction(pressedKey, modifiers);
    }

    /// <summary>
    /// Parse a single key string into a Keys enum value
    /// </summary>
    private static Keys ParseSingleKey(string keyString)
    {
        // Handle common aliases and special cases
        var normalizedKey = keyString switch
        {
            "CTRL" or "CONTROL"     => "LeftControl",
            "ALT"                   => "LeftAlt",
            "SHIFT"                 => "LeftShift",
            "ESC" or "ESCAPE"       => "Escape",
            "ENTER" or "RETURN"     => "Enter",
            "SPACE" or "SPACEBAR"   => "Space",
            "DEL" or "DELETE"       => "Delete",
            "INS" or "INSERT"       => "Insert",
            "PGUP" or "PAGEUP"      => "PageUp",
            "PGDN" or "PAGEDOWN"    => "PageDown",
            "HOME"                  => "Home",
            "END"                   => "End",
            "UP" or "UPARROW"       => "Up",
            "DOWN" or "DOWNARROW"   => "Down",
            "LEFT" or "LEFTARROW"   => "Left",
            "RIGHT" or "RIGHTARROW" => "Right",
            "TAB"                   => "Tab",
            "BACKSPACE" or "BACK"   => "Back",
            _                       => keyString
        };

        // Try to parse as Keys enum
        if (Enum.TryParse<Keys>(normalizedKey, true, out var parsedKey))
        {
            return parsedKey;
        }

        // Handle single character keys (A-Z, 0-9)
        if (normalizedKey.Length == 1)
        {
            var ch = normalizedKey[0];
            if (ch >= 'A' && ch <= 'Z')
            {
                return (Keys)ch; // ASCII values align with Keys enum for A-Z
            }

            if (ch >= '0' && ch <= '9')
            {
                return (Keys)ch; // ASCII values align with Keys enum for 0-9
            }
        }

        throw new ArgumentException($"Unknown key: '{keyString}'");
    }

    /// <summary>
    /// Check if a key is a modifier key
    /// </summary>
    private static bool IsModifierKey(Keys key)
    {
        return key switch
        {
            Keys.LeftControl or Keys.RightControl or
                Keys.LeftAlt or Keys.RightAlt or
                Keys.LeftShift or Keys.RightShift or
                Keys.LeftWindows or Keys.RightWindows => true,
            _ => false
        };
    }
}
