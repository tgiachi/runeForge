namespace Runeforge.Engine.Types.Tick;

/// <summary>
/// Result of action execution
/// </summary>
public enum ActionResult
{
    Success,
    Failed,
    Blocked,
    Invalid,
    Cancelled
}
