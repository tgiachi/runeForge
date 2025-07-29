using Runeforge.Engine.Interfaces.Ticks;
using Runeforge.Engine.Ticks.Actions;
using Runeforge.Engine.Types.Tick;

namespace Runeforge.Tests.Services;

/// <summary>
/// Test implementation of BaseAction for testing purposes
/// </summary>
public class TestSingleAction : BaseAction
{
    public override ActionPriority Priority { get; }
    public override int Speed { get; set; }

    public bool WasExecuteActionCalled { get; private set; }
    public bool WasCanBeExecutedCalled { get; private set; }
    public bool WasOnExecutionErrorCalled { get; private set; }
    public ActionResult ResultToReturn { get; set; } = ActionResult.Success;
    public bool ShouldThrowException { get; set; } = false;
    public Exception? ThrownException { get; private set; }
    public string? CustomDescription { get; set; }

    public TestSingleAction(ActionPriority priority = ActionPriority.Normal, int speed = 100)
    {
        Priority = priority;
        Speed = speed;
    }

    public override bool CanBeExecuted()
    {
        WasCanBeExecutedCalled = true;
        return base.CanBeExecuted();
    }

    protected override ActionResult ExecuteAction()
    {
        WasExecuteActionCalled = true;

        if (ShouldThrowException)
        {
            throw new InvalidOperationException("Test exception");
        }

        return ResultToReturn;
    }

    protected override void OnExecutionError(Exception exception)
    {
        WasOnExecutionErrorCalled = true;
        ThrownException = exception;
        base.OnExecutionError(exception);
    }

    public override string GetDescription()
    {
        return CustomDescription ?? base.GetDescription();
    }

    public void Reset()
    {
        WasExecuteActionCalled = false;
        WasCanBeExecutedCalled = false;
        WasOnExecutionErrorCalled = false;
        ThrownException = null;
    }
}

/// <summary>
/// Test implementation of MultiTickAction for testing purposes
/// </summary>
public class TestMultiTickAction : MultiTickAction
{
    public override ActionPriority Priority { get; }
    public override int Speed { get; set; }

    public int ExecuteTickCallCount { get; private set; }
    public List<bool> WasFirstTick { get; } = new();
    public List<bool> WasLastTick { get; } = new();
    public List<double> ProgressValues { get; } = new();
    public ActionResult ResultToReturn { get; set; } = ActionResult.Success;
    public bool ShouldFailOnTick { get; set; } = false;
    public int FailOnTickNumber { get; set; } = -1;

    public TestMultiTickAction(int totalTicks, ActionPriority priority = ActionPriority.Normal, int speed = 100)
        : base(totalTicks)
    {
        Priority = priority;
        Speed = speed;
    }

    protected override ActionResult ExecuteTick()
    {
        ExecuteTickCallCount++;
        WasFirstTick.Add(IsFirstTick);
        WasLastTick.Add(IsLastTick);

        // Capture progress BEFORE decrementing (this matches the actual MultiTickAction behavior)
        ProgressValues.Add(Progress);

        if (ShouldFailOnTick && ExecuteTickCallCount == FailOnTickNumber)
        {
            return ActionResult.Failed;
        }

        return ResultToReturn;
    }

    public void Reset()
    {
        ExecuteTickCallCount = 0;
        WasFirstTick.Clear();
        WasLastTick.Clear();
        ProgressValues.Clear();
    }
}

/// <summary>
/// Tests for BaseAction abstract class
/// </summary>
[TestFixture]
public class BaseActionTests
{
    private TestSingleAction _testAction;

    [SetUp]
    public void Setup()
    {
        _testAction = new TestSingleAction();
    }

    [Test]
    public void Constructor_ShouldGenerateUniqueId()
    {
        // Arrange & Act
        var action1 = new TestSingleAction();
        var action2 = new TestSingleAction();

        // Assert
        Assert.That(action1.Id, Is.Not.EqualTo(action2.Id));
        Assert.That(action1.Id, Is.Not.EqualTo(Guid.Empty));
        Assert.That(action2.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Constructor_ShouldSetPriorityAndSpeed()
    {
        // Arrange & Act
        var action = new TestSingleAction(ActionPriority.High, 150);

        // Assert
        Assert.That(action.Priority, Is.EqualTo(ActionPriority.High));
        Assert.That(action.Speed, Is.EqualTo(150));
    }

    [Test]
    public void CanBeExecuted_DefaultImplementation_ShouldReturnTrue()
    {
        // Act
        var result = _testAction.CanBeExecuted();

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_testAction.WasCanBeExecutedCalled, Is.True);
    }

    [Test]
    public void Execute_ShouldCallExecuteAction()
    {
        // Act
        var result = _testAction.Execute();

        // Assert
        Assert.That(result, Is.EqualTo(ActionResult.Success));
        Assert.That(_testAction.WasExecuteActionCalled, Is.True);
    }

    [Test]
    public void Execute_WithDifferentResults_ShouldReturnCorrectResult()
    {
        // Test each possible result
        var results = new[]
        {
            ActionResult.Success,
            ActionResult.Failed,
            ActionResult.Blocked,
            ActionResult.Invalid,
            ActionResult.Cancelled
        };

        foreach (var expectedResult in results)
        {
            // Arrange
            _testAction.Reset();
            _testAction.ResultToReturn = expectedResult;

            // Act
            var actualResult = _testAction.Execute();

            // Assert
            Assert.That(actualResult, Is.EqualTo(expectedResult),
                       $"Expected {expectedResult} but got {actualResult}");
        }
    }

    [Test]
    public void Execute_WithException_ShouldReturnFailedAndCallErrorHandler()
    {
        // Arrange
        _testAction.ShouldThrowException = true;

        // Act
        var result = _testAction.Execute();

        // Assert
        Assert.That(result, Is.EqualTo(ActionResult.Failed));
        Assert.That(_testAction.WasExecuteActionCalled, Is.True);
        Assert.That(_testAction.WasOnExecutionErrorCalled, Is.True);
        Assert.That(_testAction.ThrownException, Is.Not.Null);
        Assert.That(_testAction.ThrownException.Message, Is.EqualTo("Test exception"));
    }

    [Test]
    public void GetDescription_DefaultImplementation_ShouldReturnClassName()
    {
        // Act
        var description = _testAction.GetDescription();

        // Assert
        Assert.That(description, Is.EqualTo("TestSingleAction"));
    }

    [Test]
    public void GetDescription_WithCustomDescription_ShouldReturnCustom()
    {
        // Arrange
        _testAction.CustomDescription = "Custom test action";

        // Act
        var description = _testAction.GetDescription();

        // Assert
        Assert.That(description, Is.EqualTo("Custom test action"));
    }

    [Test]
    public void ToString_ShouldContainAllRelevantInfo()
    {
        // Arrange
        var action = new TestSingleAction(ActionPriority.High, 150);

        // Act
        var result = action.ToString();

        // Assert
        Assert.That(result, Does.Contain("TestSingleAction"));
        Assert.That(result, Does.Contain(action.Id.ToString()));
        Assert.That(result, Does.Contain("High"));
        Assert.That(result, Does.Contain("150"));
    }

    [Test]
    public void Speed_ShouldBeSettable()
    {
        // Arrange
        var action = new TestSingleAction(speed: 100);

        // Act
        action.Speed = 200;

        // Assert
        Assert.That(action.Speed, Is.EqualTo(200));
    }
}

/// <summary>
/// Tests for MultiTickAction abstract class
/// </summary>
[TestFixture]
public class MultiTickActionTests
{
    private TestMultiTickAction _testAction;

    [SetUp]
    public void Setup()
    {
        _testAction = new TestMultiTickAction(3); // 3 tick action
    }

    [Test]
    public void Constructor_ShouldInitializeCorrectly()
    {
        // Arrange & Act
        var action = new TestMultiTickAction(5, ActionPriority.Low, 75);

        // Assert
        Assert.That(action.TotalTicks, Is.EqualTo(5));
        Assert.That(action.RemainingTicks, Is.EqualTo(5));
        Assert.That(action.Priority, Is.EqualTo(ActionPriority.Low));
        Assert.That(action.Speed, Is.EqualTo(75));
        Assert.That(action.Progress, Is.EqualTo(0.0));
    }

    [Test]
    public void Execute_FirstTick_ShouldSetupCorrectly()
    {
        // Act
        var result = _testAction.Execute();

        // Assert
        Assert.That(result, Is.EqualTo(ActionResult.Continuing));
        Assert.That(_testAction.ExecuteTickCallCount, Is.EqualTo(1));
        Assert.That(_testAction.RemainingTicks, Is.EqualTo(2));
        Assert.That(_testAction.WasFirstTick[0], Is.True);
        Assert.That(_testAction.WasLastTick[0], Is.False);
        // Progress is captured BEFORE decrementing, so first tick shows 0.0
        Assert.That(_testAction.ProgressValues[0], Is.EqualTo(0.0).Within(0.001));
    }

    [Test]
    public void Execute_MiddleTick_ShouldContinue()
    {
        // Arrange - Execute first tick
        _testAction.Execute();

        // Act - Execute second tick
        var result = _testAction.Execute();

        // Assert
        Assert.That(result, Is.EqualTo(ActionResult.Continuing));
        Assert.That(_testAction.ExecuteTickCallCount, Is.EqualTo(2));
        Assert.That(_testAction.RemainingTicks, Is.EqualTo(1));
        Assert.That(_testAction.WasFirstTick[1], Is.False);
        Assert.That(_testAction.WasLastTick[1], Is.False);
        // Progress is captured BEFORE decrementing, so second tick shows 1/3
        Assert.That(_testAction.ProgressValues[1], Is.EqualTo(1.0/3.0).Within(0.001));
    }

    [Test]
    public void Execute_LastTick_ShouldComplete()
    {
        // Arrange - Execute first two ticks
        _testAction.Execute(); // Tick 1
        _testAction.Execute(); // Tick 2

        // Act - Execute final tick
        var result = _testAction.Execute();

        // Assert
        Assert.That(result, Is.EqualTo(ActionResult.Success));
        Assert.That(_testAction.ExecuteTickCallCount, Is.EqualTo(3));
        Assert.That(_testAction.RemainingTicks, Is.EqualTo(0));
        Assert.That(_testAction.WasFirstTick[2], Is.False);
        Assert.That(_testAction.WasLastTick[2], Is.True);
        // Progress is captured BEFORE decrementing, so last tick shows 2/3
        Assert.That(_testAction.ProgressValues[2], Is.EqualTo(2.0/3.0).Within(0.001));
    }

    [Test]
    public void Execute_WithFailureInMiddle_ShouldStopExecution()
    {
        // Arrange
        _testAction.ShouldFailOnTick = true;
        _testAction.FailOnTickNumber = 2;

        // Act
        var result1 = _testAction.Execute(); // Tick 1 - should succeed
        var result2 = _testAction.Execute(); // Tick 2 - should fail

        // Assert
        Assert.That(result1, Is.EqualTo(ActionResult.Continuing));
        Assert.That(result2, Is.EqualTo(ActionResult.Failed));
        Assert.That(_testAction.ExecuteTickCallCount, Is.EqualTo(2));
        // When ExecuteTick returns Failed, the main Execute method doesn't decrement RemainingTicks
        Assert.That(_testAction.RemainingTicks, Is.EqualTo(2));
    }

    [Test]
    public void Execute_CompleteCycle_ShouldTrackProgressCorrectly()
    {
        // Act - Execute complete 3-tick cycle
        var results = new List<ActionResult>();
        for (int i = 0; i < 3; i++)
        {
            results.Add(_testAction.Execute());
        }

        // Assert
        Assert.That(results, Is.EqualTo(new[]
        {
            ActionResult.Continuing,
            ActionResult.Continuing,
            ActionResult.Success
        }));

        // Progress is captured BEFORE decrementing each tick
        Assert.That(_testAction.ProgressValues, Is.EqualTo(new[] { 0.0, 1.0/3.0, 2.0/3.0 }).Within(0.001));
        Assert.That(_testAction.WasFirstTick, Is.EqualTo(new[] { true, false, false }));
        Assert.That(_testAction.WasLastTick, Is.EqualTo(new[] { false, false, true }));
    }

    [Test]
    public void Execute_SingleTickAction_ShouldCompleteImmediately()
    {
        // Arrange
        var singleTickAction = new TestMultiTickAction(1);

        // Act
        var result = singleTickAction.Execute();

        // Assert
        Assert.That(result, Is.EqualTo(ActionResult.Success));
        Assert.That(singleTickAction.ExecuteTickCallCount, Is.EqualTo(1));
        Assert.That(singleTickAction.RemainingTicks, Is.EqualTo(0));
        Assert.That(singleTickAction.WasFirstTick[0], Is.True);
        Assert.That(singleTickAction.WasLastTick[0], Is.True);
        Assert.That(singleTickAction.Progress, Is.EqualTo(1.0));
    }

    [Test]
    public void Progress_ShouldCalculateCorrectly()
    {
        // Arrange
        var action = new TestMultiTickAction(4); // 4 ticks

        // Act & Assert
        Assert.That(action.Progress, Is.EqualTo(0.0));

        action.Execute(); // After tick 1
        Assert.That(action.Progress, Is.EqualTo(0.25).Within(0.001));

        action.Execute(); // After tick 2
        Assert.That(action.Progress, Is.EqualTo(0.5).Within(0.001));

        action.Execute(); // After tick 3
        Assert.That(action.Progress, Is.EqualTo(0.75).Within(0.001));

        action.Execute(); // After tick 4
        Assert.That(action.Progress, Is.EqualTo(1.0).Within(0.001));
    }

    [Test]
    public void IsFirstTick_ShouldBeTrueOnlyForFirstExecution()
    {
        // Arrange
        var action = new TestMultiTickAction(3);

        // Act & Assert
        Assert.That(action.IsFirstTick, Is.True, "Should be first tick before any execution");

        action.Execute();
        Assert.That(action.IsFirstTick, Is.False, "Should not be first tick after first execution");

        action.Execute();
        Assert.That(action.IsFirstTick, Is.False, "Should not be first tick after second execution");
    }

    [Test]
    public void IsLastTick_ShouldBeTrueOnlyForLastExecution()
    {
        // Arrange
        var action = new TestMultiTickAction(3);

        // Act & Assert
        Assert.That(action.IsLastTick, Is.False, "Should not be last tick initially");

        action.Execute(); // Tick 1
        Assert.That(action.IsLastTick, Is.False, "Should not be last tick after first execution");

        action.Execute(); // Tick 2
        Assert.That(action.IsLastTick, Is.True, "Should be last tick before final execution");

        action.Execute(); // Tick 3
        Assert.That(action.IsLastTick, Is.False, "Should not be last tick after completion");
    }

    [Test]
    public void Speed_ShouldBeSettable()
    {
        // Arrange
        var action = new TestMultiTickAction(2, speed: 100);

        // Act
        action.Speed = 150;

        // Assert
        Assert.That(action.Speed, Is.EqualTo(150));
    }
}

/// <summary>
/// Integration tests combining both types of actions
/// </summary>
[TestFixture]
public class ActionIntegrationTests
{
    [Test]
    public void MultipleActions_ShouldMaintainUniqueIds()
    {
        // Arrange
        var actions = new List<ITickAction>
        {
            new TestSingleAction(ActionPriority.Normal),
            new TestSingleAction(ActionPriority.High),
            new TestMultiTickAction(3, ActionPriority.Normal),
            new TestMultiTickAction(2, ActionPriority.Low),
            new TestSingleAction(ActionPriority.Instant)
        };

        // Act
        var ids = actions.Select(a => a.Id).ToList();

        // Assert
        Assert.That(ids.Distinct().Count(), Is.EqualTo(actions.Count),
                   "All actions should have unique IDs");

        // Verify no empty GUIDs
        Assert.That(ids.All(id => id != Guid.Empty), Is.True,
                   "No action should have an empty GUID");
    }

    [Test]
    public void ActionsWithDifferentPriorities_ShouldSortCorrectly()
    {
        // Arrange
        var actions = new List<ITickAction>
        {
            new TestSingleAction(ActionPriority.Normal),
            new TestSingleAction(ActionPriority.Instant),
            new TestSingleAction(ActionPriority.Low),
            new TestSingleAction(ActionPriority.High),
            new TestSingleAction(ActionPriority.Environmental)
        };

        // Act
        var sortedActions = actions
            .OrderBy(a => (int)a.Priority)
            .ThenByDescending(a => a.Speed)
            .ThenBy(a => a.Id)
            .ToList();

        // Assert
        Assert.That(sortedActions[0].Priority, Is.EqualTo(ActionPriority.Instant));
        Assert.That(sortedActions[1].Priority, Is.EqualTo(ActionPriority.High));
        Assert.That(sortedActions[2].Priority, Is.EqualTo(ActionPriority.Normal));
        Assert.That(sortedActions[3].Priority, Is.EqualTo(ActionPriority.Low));
        Assert.That(sortedActions[4].Priority, Is.EqualTo(ActionPriority.Environmental));
    }

    [Test]
    public void ActionsWithSameePriorityDifferentSpeed_ShouldSortBySpeedDescending()
    {
        // Arrange
        var actions = new List<ITickAction>
        {
            new TestSingleAction(ActionPriority.Normal, 50),
            new TestSingleAction(ActionPriority.Normal, 200),
            new TestSingleAction(ActionPriority.Normal, 100),
            new TestSingleAction(ActionPriority.Normal, 150)
        };

        // Act
        var sortedActions = actions
            .OrderBy(a => (int)a.Priority)
            .ThenByDescending(a => a.Speed)
            .ToList();

        // Assert
        var speeds = sortedActions.Select(a => a.Speed).ToArray();
        Assert.That(speeds, Is.EqualTo(new[] { 200, 150, 100, 50 }));
    }

    [Test]
    public void MixedSingleAndMultiTickActions_ShouldExecuteCorrectly()
    {
        // Arrange
        var singleTickAction = new TestSingleAction();
        var multiTickAction = new TestMultiTickAction(2);

        // Act
        var singleResult = singleTickAction.Execute();
        var multiResult1 = multiTickAction.Execute();
        var multiResult2 = multiTickAction.Execute();

        // Assert
        Assert.That(singleResult, Is.EqualTo(ActionResult.Success));
        Assert.That(multiResult1, Is.EqualTo(ActionResult.Continuing));
        Assert.That(multiResult2, Is.EqualTo(ActionResult.Success));

        // Verify execution tracking
        Assert.That(singleTickAction.WasExecuteActionCalled, Is.True);
        Assert.That(multiTickAction.ExecuteTickCallCount, Is.EqualTo(2));
    }

    [Test]
    public void ActionExecution_ShouldNotAffectOtherActions()
    {
        // Arrange
        var action1 = new TestSingleAction();
        var action2 = new TestSingleAction();
        var action3 = new TestMultiTickAction(2);

        action1.ShouldThrowException = true; // This one will fail

        // Act
        var result1 = action1.Execute(); // Should fail
        var result2 = action2.Execute(); // Should succeed
        var result3 = action3.Execute(); // Should continue

        // Assert
        Assert.That(result1, Is.EqualTo(ActionResult.Failed));
        Assert.That(result2, Is.EqualTo(ActionResult.Success));
        Assert.That(result3, Is.EqualTo(ActionResult.Continuing));

        // Verify independent execution
        Assert.That(action1.WasOnExecutionErrorCalled, Is.True);
        Assert.That(action2.WasExecuteActionCalled, Is.True);
        Assert.That(action2.WasOnExecutionErrorCalled, Is.False);
        Assert.That(action3.ExecuteTickCallCount, Is.EqualTo(1));
    }

    [Test]
    public void ActionReset_ShouldClearAllTrackingData()
    {
        // Arrange
        var singleAction = new TestSingleAction();
        var multiAction = new TestMultiTickAction(3);

        // Execute actions to populate data
        singleAction.Execute();
        multiAction.Execute();
        multiAction.Execute();

        // Act
        singleAction.Reset();
        multiAction.Reset();

        // Assert
        Assert.That(singleAction.WasExecuteActionCalled, Is.False);
        Assert.That(singleAction.WasCanBeExecutedCalled, Is.False);

        Assert.That(multiAction.ExecuteTickCallCount, Is.EqualTo(0));
        Assert.That(multiAction.WasFirstTick, Is.Empty);
        Assert.That(multiAction.WasLastTick, Is.Empty);
        Assert.That(multiAction.ProgressValues, Is.Empty);
    }
}
