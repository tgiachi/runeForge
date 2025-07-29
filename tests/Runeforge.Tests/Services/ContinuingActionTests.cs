using Runeforge.Engine.Services;
using Runeforge.Engine.Ticks.Actions;
using Runeforge.Engine.Types.Tick;

namespace Runeforge.Tests.Services;

[TestFixture]
public class ContinuingActionTests
{
    private TickSystemService _tickSystemService;

    [SetUp]
    public void Setup()
    {
        _tickSystemService = new TickSystemService();
    }

    [Test]
    public void ExecuteTick_WithContinuingAction_ShouldRequeueAction()
    {
        // Arrange
        var continuingAction = new TestContinuingAction(3); // 3 tick duration
        _tickSystemService.EnqueueAction(continuingAction);

        // Act - Execute first tick
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(
            _tickSystemService.ContinuingActionsCount,
            Is.EqualTo(1),
            "Action should be continuing after first tick"
        );
        Assert.That(continuingAction.ExecuteCallCount, Is.EqualTo(1));
    }

    [Test]
    public void ExecuteTick_WithMultiTickAction_ShouldCompleteAfterAllTicks()
    {
        // Arrange
        var action = new TestContinuingAction(3);
        _tickSystemService.EnqueueAction(action);

        // Act - Execute all ticks
        _tickSystemService.ExecuteTick(); // Tick 1
        _tickSystemService.ExecuteTick(); // Tick 2
        _tickSystemService.ExecuteTick(); // Tick 3

        // Assert
        Assert.That(
            _tickSystemService.ContinuingActionsCount,
            Is.EqualTo(0),
            "No actions should be continuing after completion"
        );
        Assert.That(action.ExecuteCallCount, Is.EqualTo(3));
        Assert.That(action.IsCompleted, Is.True);
    }

    [Test]
    public void ClearContinuingActions_ShouldRemoveAllContinuingActions()
    {
        // Arrange
        var action1 = new TestContinuingAction(5);
        var action2 = new TestContinuingAction(3);

        _tickSystemService.EnqueueAction(action1);
        _tickSystemService.EnqueueAction(action2);
        _tickSystemService.ExecuteTick(); // Start both actions

        Assert.That(_tickSystemService.ContinuingActionsCount, Is.EqualTo(2));

        // Act
        _tickSystemService.ClearContinuingActions();

        // Assert
        Assert.That(_tickSystemService.ContinuingActionsCount, Is.EqualTo(0));
    }
}

// Test action che supporta continuing
public class TestContinuingAction : MultiTickAction
{
    public override ActionPriority Priority => ActionPriority.Normal;
    public override int Speed { get; set; } = 100;

    public int ExecuteCallCount { get; private set; }
    public bool IsCompleted { get; private set; }

    public TestContinuingAction(int duration) : base(duration)
    {
    }

    protected override ActionResult ExecuteTick()
    {
        ExecuteCallCount++;

        if (IsLastTick)
        {
            IsCompleted = true;
        }

        return ActionResult.Success;
    }
}
