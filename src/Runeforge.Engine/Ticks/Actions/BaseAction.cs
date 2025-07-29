using Runeforge.Engine.Interfaces.Ticks;
using Runeforge.Engine.Types.Tick;

namespace Runeforge.Engine.Ticks.Actions;

// <summary>
/// Base abstract class for single-tick actions
/// </summary>
public abstract class BaseAction : ITickAction
{
    public Guid Id { get; } = Guid.NewGuid();
    public abstract ActionPriority Priority { get; }
    public abstract int Speed { get; set; }

    /// <summary>
    /// Default implementation - most actions can be executed unless overridden
    /// </summary>
    public virtual bool CanBeExecuted() => true;

    /// <summary>
    /// Execute the action - calls the abstract ExecuteAction method
    /// </summary>
    public ActionResult Execute()
    {
        try
        {
            return ExecuteAction();
        }
        catch (Exception ex)
        {
            OnExecutionError(ex);
            return ActionResult.Failed;
        }
    }

    /// <summary>
    /// Implement this method to define what the action does
    /// </summary>
    /// <returns>Result of the action execution</returns>
    protected abstract ActionResult ExecuteAction();

    /// <summary>
    /// Called when an exception occurs during execution
    /// Override to provide custom error handling
    /// </summary>
    /// <param name="exception">The exception that occurred</param>
    protected virtual void OnExecutionError(Exception exception)
    {

    }

    /// <summary>
    /// Get a human-readable description of this action
    /// </summary>
    /// <returns>Description string</returns>
    public virtual string GetDescription() => GetType().Name;

    /// <summary>
    /// Override ToString for better debugging
    /// </summary>
    public override string ToString() =>
        $"{GetType().Name}(Id: {Id}, Priority: {Priority}, Speed: {Speed})";
}
