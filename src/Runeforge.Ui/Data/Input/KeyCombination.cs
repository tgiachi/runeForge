using SadConsole.Input;

namespace Runeforge.Ui.Data.Input;

/// <summary>
/// Represents a parsed key combination with modifiers and main key
/// </summary>
public readonly struct KeyCombination : IEquatable<KeyCombination>
{
    public Keys MainKey { get; }
    public HashSet<Keys> Modifiers { get; }

    public string Action { get; }

    public KeyCombination(Keys mainKey, string action,  HashSet<Keys>? modifiers = null)
    {
        MainKey = mainKey;
        Action = action;
        Modifiers = modifiers ?? [];
    }

    public bool Equals(KeyCombination other)
    {
        return MainKey == other.MainKey && Modifiers.SetEquals(other.Modifiers);
    }

    public override bool Equals(object? obj) => obj is KeyCombination other && Equals(other);

    public override int GetHashCode()
    {
        var hash = MainKey.GetHashCode();

        return Modifiers.OrderBy(m => m).Aggregate(hash, (current, modifier) => current ^ modifier.GetHashCode());
    }

    public override string ToString()
    {
        var parts = new List<string>();
        if (Modifiers.Contains(Keys.LeftControl) || Modifiers.Contains(Keys.RightControl))
        {
            parts.Add("CTRL");
        }

        if (Modifiers.Contains(Keys.LeftAlt) || Modifiers.Contains(Keys.RightAlt))
        {
            parts.Add("ALT");
        }

        if (Modifiers.Contains(Keys.LeftShift) || Modifiers.Contains(Keys.RightShift))
        {
            parts.Add("SHIFT");
        }

        parts.Add(MainKey.ToString().ToUpper());
        return string.Join("+", parts);
    }
}
