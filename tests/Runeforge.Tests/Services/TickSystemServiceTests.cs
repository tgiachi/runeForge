using NUnit.Framework;
using Runeforge.Engine.Services;
using Runeforge.Engine.Interfaces.Services;
using Runeforge.Engine.Ticks;
using System.Diagnostics;
using Runeforge.Engine.Interfaces.Ticks;
using Runeforge.Engine.Types.Tick;

namespace Runeforge.Tests.Services;

/// <summary>
/// Mock action for testing purposes
/// </summary>
public class MockTickAction : ITickAction
{
    public Guid Id { get; } = Guid.NewGuid();
    public ActionPriority Priority { get; set; } = ActionPriority.Normal;
    public int Speed { get; set; } = 100;

    public bool CanExecuteResult { get; set; } = true;
    public ActionResult ExecuteResult { get; set; } = ActionResult.Success;
    public bool WasCanExecuteCalled { get; private set; }
    public bool WasExecuteCalled { get; private set; }
    public int ExecuteCallCount { get; private set; }

    public bool CanBeExecuted()
    {
        WasCanExecuteCalled = true;
        return CanExecuteResult;
    }

    public ActionResult Execute()
    {
        WasExecuteCalled = true;
        ExecuteCallCount++;
        return ExecuteResult;
    }

    public void Reset()
    {
        WasCanExecuteCalled = false;
        WasExecuteCalled = false;
        ExecuteCallCount = 0;
    }
}

/// <summary>
/// Action that throws exception for testing error handling
/// </summary>
public class ExceptionThrowingAction : ITickAction
{
    public Guid Id { get; } = Guid.NewGuid();
    public ActionPriority Priority { get; set; } = ActionPriority.Normal;
    public int Speed { get; set; } = 100;

    public bool CanBeExecuted() => true;

    public ActionResult Execute()
    {
        throw new InvalidOperationException("Test exception");
    }
}

/// <summary>
/// Tests for TickSystemService
/// </summary>
[TestFixture]
public class TickSystemServiceTests
{
    private TickSystemService _tickSystemService;
    private List<int> _tickStartedCalls;
    private List<int> _tickCalls;
    private List<int> _tickEndedCalls;

    [SetUp]
    public void Setup()
    {
        _tickSystemService = new TickSystemService();
        _tickStartedCalls = new List<int>();
        _tickCalls = new List<int>();
        _tickEndedCalls = new List<int>();

        // Subscribe to events for testing
        _tickSystemService.TickStarted += OnTickStarted;
        _tickSystemService.Tick += OnTick;
        _tickSystemService.TickEnded += OnTickEnded;
    }

    [TearDown]
    public void TearDown()
    {
        // Unsubscribe from events
        _tickSystemService.TickStarted -= OnTickStarted;
        _tickSystemService.Tick -= OnTick;
        _tickSystemService.TickEnded -= OnTickEnded;
    }

    private void OnTickStarted(int tickCount) => _tickStartedCalls.Add(tickCount);
    private void OnTick(int tickCount) => _tickCalls.Add(tickCount);
    private void OnTickEnded(int tickCount) => _tickEndedCalls.Add(tickCount);

    [Test]
    public void Constructor_ShouldInitializeWithZeroTickCount()
    {
        // Arrange & Act
        var service = new TickSystemService();

        // Assert
        Assert.That(service.TickCount, Is.EqualTo(0));
    }

    [Test]
    public void ExecuteTick_WithNoActions_ShouldIncrementTickCountAndFireEvents()
    {
        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(_tickSystemService.TickCount, Is.EqualTo(1));
        Assert.That(_tickStartedCalls, Is.EqualTo(new[] { 1 }));
        Assert.That(_tickCalls, Is.EqualTo(new[] { 1 }));
        Assert.That(_tickEndedCalls, Is.EqualTo(new[] { 1 }));
    }

    [Test]
    public void ExecuteTick_MultipleTimes_ShouldIncrementTickCountSequentially()
    {
        // Act
        _tickSystemService.ExecuteTick();
        _tickSystemService.ExecuteTick();
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(_tickSystemService.TickCount, Is.EqualTo(3));
        Assert.That(_tickStartedCalls, Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(_tickCalls, Is.EqualTo(new[] { 1, 2, 3 }));
        Assert.That(_tickEndedCalls, Is.EqualTo(new[] { 1, 2, 3 }));
    }

    [Test]
    public void ExecuteTick_ShouldFireEventsInCorrectOrder()
    {
        // Arrange
        var eventOrder = new List<string>();

        _tickSystemService.TickStarted += _ => eventOrder.Add("TickStarted");
        _tickSystemService.Tick += _ => eventOrder.Add("Tick");
        _tickSystemService.TickEnded += _ => eventOrder.Add("TickEnded");

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(eventOrder, Is.EqualTo(new[] { "TickStarted", "Tick", "TickEnded" }));
    }

    [Test]
    public void ExecuteTick_WithSingleAction_ShouldExecuteAction()
    {
        // Arrange
        var mockAction = new MockTickAction();

        // We need to access the action queue - let's add a method to add actions
        // For now, we'll use reflection to access the private field
        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;
        actionQueue.Enqueue(mockAction);

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(mockAction.WasCanExecuteCalled, Is.True);
        Assert.That(mockAction.WasExecuteCalled, Is.True);
        Assert.That(mockAction.ExecuteCallCount, Is.EqualTo(1));
    }

    [Test]
    public void ExecuteTick_WithMultipleActions_ShouldExecuteAllActions()
    {
        // Arrange
        var action1 = new MockTickAction { Priority = ActionPriority.High, Speed = 150 };
        var action2 = new MockTickAction { Priority = ActionPriority.Normal, Speed = 100 };
        var action3 = new MockTickAction { Priority = ActionPriority.Low, Speed = 50 };

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;

        actionQueue.Enqueue(action2); // Add in different order to test sorting
        actionQueue.Enqueue(action1);
        actionQueue.Enqueue(action3);

        // Act
        _tickSystemService.ExecuteTick();

        // Assert - All actions should be executed
        Assert.That(action1.WasExecuteCalled, Is.True, "High priority action should be executed");
        Assert.That(action2.WasExecuteCalled, Is.True, "Normal priority action should be executed");
        Assert.That(action3.WasExecuteCalled, Is.True, "Low priority action should be executed");
    }

    [Test]
    public void ExecuteTick_WithActionThatCannotExecute_ShouldSkipAction()
    {
        // Arrange
        var mockAction = new MockTickAction { CanExecuteResult = false };

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;
        actionQueue.Enqueue(mockAction);

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(mockAction.WasCanExecuteCalled, Is.True);
        Assert.That(mockAction.WasExecuteCalled, Is.False, "Execute should not be called when CanBeExecuted returns false");
    }

    [Test]
    public void ExecuteTick_WithActionThatThrowsException_ShouldContinueExecution()
    {
        // Arrange
        var exceptionAction = new ExceptionThrowingAction();
        var normalAction = new MockTickAction();

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;
        actionQueue.Enqueue(exceptionAction);
        actionQueue.Enqueue(normalAction);

        // Act & Assert - Should not throw exception
        Assert.DoesNotThrow(() => _tickSystemService.ExecuteTick());

        // Normal action should still execute despite exception in previous action
        Assert.That(normalAction.WasExecuteCalled, Is.True);
        Assert.That(_tickSystemService.TickCount, Is.EqualTo(1));
    }

    [Test]
    public void ExecuteTick_ShouldClearActionQueueAfterExecution()
    {
        // Arrange
        var mockAction = new MockTickAction();

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;
        actionQueue.Enqueue(mockAction);

        Assert.That(actionQueue.Count, Is.EqualTo(1), "Action should be in queue before execution");

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(actionQueue.Count, Is.EqualTo(0), "Action queue should be empty after execution");
    }

    [Test]
    public void ExecuteTick_WithNoEventSubscribers_ShouldNotThrow()
    {
        // Arrange
        var serviceWithoutSubscribers = new TickSystemService();

        // Act & Assert
        Assert.DoesNotThrow(() => serviceWithoutSubscribers.ExecuteTick());
        Assert.That(serviceWithoutSubscribers.TickCount, Is.EqualTo(1));
    }

    [Test]
    public void Events_ShouldReceiveCorrectTickCount()
    {
        // Act
        _tickSystemService.ExecuteTick(); // Tick 1
        _tickSystemService.ExecuteTick(); // Tick 2

        // Assert
        Assert.That(_tickStartedCalls[0], Is.EqualTo(1));
        Assert.That(_tickCalls[0], Is.EqualTo(1));
        Assert.That(_tickEndedCalls[0], Is.EqualTo(1));

        Assert.That(_tickStartedCalls[1], Is.EqualTo(2));
        Assert.That(_tickCalls[1], Is.EqualTo(2));
        Assert.That(_tickEndedCalls[1], Is.EqualTo(2));
    }

    [Test]
    public void ExecuteTick_Performance_ShouldExecuteQuickly()
    {
        // Arrange
        var actions = new List<MockTickAction>();
        for (int i = 0; i < 1000; i++)
        {
            actions.Add(new MockTickAction());
        }

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;

        foreach (var action in actions)
        {
            actionQueue.Enqueue(action);
        }

        // Act
        var stopwatch = Stopwatch.StartNew();
        _tickSystemService.ExecuteTick();
        stopwatch.Stop();

        // Assert
        Assert.That(
            stopwatch.ElapsedMilliseconds,
            Is.LessThan(100),
            "Executing 1000 simple actions should take less than 100ms"
        );

        Assert.That(
            actions.All(a => a.WasExecuteCalled),
            Is.True,
            "All actions should have been executed"
        );
    }

    [Test]
    public void ExecuteTick_EventSubscription_ShouldAllowMultipleSubscribers()
    {
        // Arrange
        var subscriber1Calls = new List<int>();
        var subscriber2Calls = new List<int>();

        _tickSystemService.Tick += tickCount => subscriber1Calls.Add(tickCount);
        _tickSystemService.Tick += tickCount => subscriber2Calls.Add(tickCount);

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(subscriber1Calls, Is.EqualTo(new[] { 1 }));
        Assert.That(subscriber2Calls, Is.EqualTo(new[] { 1 }));
    }

    [Test]
    public void ExecuteTick_EventUnsubscription_ShouldStopReceivingEvents()
    {
        // Arrange
        var calls = new List<int>();
        ITickSystemService.TickDelegate handler = tickCount => calls.Add(tickCount);

        _tickSystemService.Tick += handler;
        _tickSystemService.ExecuteTick();

        _tickSystemService.Tick -= handler;
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(
            calls,
            Is.EqualTo(new[] { 1 }),
            "Should only receive event from first tick, not second"
        );
    }
}

/// <summary>
/// Integration tests for TickSystemService with ActionQueue
/// </summary>
[TestFixture]
public class TickSystemServiceIntegrationTests
{
    private TickSystemService _tickSystemService;

    [SetUp]
    public void Setup()
    {
        _tickSystemService = new TickSystemService();
    }

    [Test]
    public void ExecuteTick_WithPriorityOrdering_ShouldExecuteInCorrectOrder()
    {
        // Arrange
        var executionOrder = new List<string>();

        var highPriorityAction = new TestTrackingAction("High", ActionPriority.Instant, executionOrder);
        var normalPriorityAction = new TestTrackingAction("Normal", ActionPriority.Normal, executionOrder);
        var lowPriorityAction = new TestTrackingAction("Low", ActionPriority.Environmental, executionOrder);

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;

        // Add in reverse priority order to test sorting
        actionQueue.Enqueue(lowPriorityAction);
        actionQueue.Enqueue(normalPriorityAction);
        actionQueue.Enqueue(highPriorityAction);

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(executionOrder, Is.EqualTo(new[] { "High", "Normal", "Low" }));
    }

    [Test]
    public void ExecuteTick_WithSpeedOrdering_ShouldExecuteFasterActionsFirst()
    {
        // Arrange
        var executionOrder = new List<string>();

        var fastAction = new TestTrackingAction("Fast", ActionPriority.Normal, executionOrder) { Speed = 200 };
        var mediumAction = new TestTrackingAction("Medium", ActionPriority.Normal, executionOrder) { Speed = 100 };
        var slowAction = new TestTrackingAction("Slow", ActionPriority.Normal, executionOrder) { Speed = 50 };

        var actionQueueField = typeof(TickSystemService).GetField(
            "_actionQueue",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        var actionQueue = (ActionQueue)actionQueueField!.GetValue(_tickSystemService)!;

        // Add in random order
        actionQueue.Enqueue(mediumAction);
        actionQueue.Enqueue(slowAction);
        actionQueue.Enqueue(fastAction);

        // Act
        _tickSystemService.ExecuteTick();

        // Assert
        Assert.That(executionOrder, Is.EqualTo(new[] { "Fast", "Medium", "Slow" }));
    }
}

/// <summary>
/// Test action that tracks execution order
/// </summary>
public class TestTrackingAction : ITickAction
{
    public Guid Id { get; } = Guid.NewGuid();
    public ActionPriority Priority { get; }
    public int Speed { get; set; } = 100;

    private readonly string _name;
    private readonly List<string> _executionOrder;

    public TestTrackingAction(string name, ActionPriority priority, List<string> executionOrder)
    {
        _name = name;
        Priority = priority;
        _executionOrder = executionOrder;
    }

    public bool CanBeExecuted() => true;

    public ActionResult Execute()
    {
        _executionOrder.Add(_name);
        return ActionResult.Success;
    }
}
