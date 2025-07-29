using Runeforge.Engine.Interfaces.Events;
using Runeforge.Engine.Services;

namespace Runeforge.Tests.Services;

/// <summary>
///     Test event for testing purposes
/// </summary>
public record TestEvent(string Message) : IEvent;

/// <summary>
///     Another test event for testing
/// </summary>
public record NumberEvent(int Value) : IEvent;

/// <summary>
///     Test handler for capturing events
/// </summary>
public class TestEventHandler : IEventHandler<TestEvent>
{
    public List<TestEvent> ReceivedEvents { get; } = new();

    public void Handle(TestEvent eventData)
    {
        ReceivedEvents.Add(eventData);
    }
}

/// <summary>
///     Number event handler for testing
/// </summary>
public class NumberEventHandler : IEventHandler<NumberEvent>
{
    public List<NumberEvent> ReceivedEvents { get; } = new();

    public void Handle(NumberEvent eventData)
    {
        ReceivedEvents.Add(eventData);
    }
}

[TestFixture]
public class EventBusServiceTests
{
    [SetUp]
    public void Setup()
    {
        _eventBusService = new EventBusService();
    }

    [TearDown]
    public void TearDown()
    {
        _eventBusService?.Clear();
    }

    private EventBusService _eventBusService;

    [Test]
    public void Subscribe_WithValidHandler_ShouldAddHandlerSuccessfully()
    {
        // Arrange
        var handler = new TestEventHandler();

        // Act
        _eventBusService.Subscribe(handler);

        // Assert
        Assert.That(_eventBusService.HasSubscribers<TestEvent>(), Is.True);
        Assert.That(_eventBusService.GetSubscriberCount<TestEvent>(), Is.EqualTo(1));
    }

    [Test]
    public void Subscribe_WithNullHandler_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _eventBusService.Subscribe<TestEvent>(null));
    }

    [Test]
    public void Publish_WithSubscribedHandler_ShouldCallHandler()
    {
        // Arrange
        var handler = new TestEventHandler();
        var testEvent = new TestEvent("Test Message");

        _eventBusService.Subscribe(handler);

        // Act
        _eventBusService.Publish(testEvent);

        // Assert
        Assert.That(handler.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(handler.ReceivedEvents[0].Message, Is.EqualTo("Test Message"));
    }

    [Test]
    public async Task PublishAsync_WithSubscribedHandler_ShouldCallHandler()
    {
        // Arrange
        var handler = new TestEventHandler();
        var testEvent = new TestEvent("Async Test Message");

        _eventBusService.Subscribe(handler);

        // Act
        await _eventBusService.PublishAsync(testEvent);

        // Wait a bit for async processing
        await Task.Delay(50);

        // Assert
        Assert.That(handler.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(handler.ReceivedEvents[0].Message, Is.EqualTo("Async Test Message"));
    }

    [Test]
    public void Publish_WithNullEvent_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => _eventBusService.Publish<TestEvent>(null));
    }

    [Test]
    public void Unsubscribe_WithSubscribedHandler_ShouldRemoveHandler()
    {
        // Arrange
        var handler = new TestEventHandler();
        _eventBusService.Subscribe(handler);

        // Act
        _eventBusService.Unsubscribe(handler);

        // Assert
        Assert.That(_eventBusService.HasSubscribers<TestEvent>(), Is.False);
        Assert.That(_eventBusService.GetSubscriberCount<TestEvent>(), Is.EqualTo(0));
    }

    [Test]
    public void Publish_WithMultipleHandlers_ShouldCallAllHandlers()
    {
        // Arrange
        var handler1 = new TestEventHandler();
        var handler2 = new TestEventHandler();
        var testEvent = new TestEvent("Multiple Handler Test");

        _eventBusService.Subscribe(handler1);
        _eventBusService.Subscribe(handler2);

        // Act
        _eventBusService.Publish(testEvent);

        // Assert
        Assert.That(handler1.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(handler2.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(_eventBusService.GetSubscriberCount<TestEvent>(), Is.EqualTo(2));
    }

    [Test]
    public void Publish_WithDifferentEventTypes_ShouldOnlyCallCorrectHandlers()
    {
        // Arrange
        var testHandler = new TestEventHandler();
        var numberHandler = new NumberEventHandler();

        _eventBusService.Subscribe(testHandler);
        _eventBusService.Subscribe(numberHandler);

        var testEvent = new TestEvent("String Event");
        var numberEvent = new NumberEvent(42);

        // Act
        _eventBusService.Publish(testEvent);
        _eventBusService.Publish(numberEvent);

        // Assert
        Assert.That(testHandler.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(numberHandler.ReceivedEvents, Has.Count.EqualTo(1));
        Assert.That(testHandler.ReceivedEvents[0].Message, Is.EqualTo("String Event"));
        Assert.That(numberHandler.ReceivedEvents[0].Value, Is.EqualTo(42));
    }

    [Test]
    public void Clear_ShouldRemoveAllHandlers()
    {
        // Arrange
        var handler1 = new TestEventHandler();
        var handler2 = new NumberEventHandler();

        _eventBusService.Subscribe(handler1);
        _eventBusService.Subscribe(handler2);

        // Act
        _eventBusService.Clear();

        // Assert
        Assert.That(_eventBusService.HasSubscribers<TestEvent>(), Is.False);
        Assert.That(_eventBusService.HasSubscribers<NumberEvent>(), Is.False);
    }

    [Test]
    public void GetStats_ShouldReturnCorrectStatistics()
    {
        // Arrange
        var handler1 = new TestEventHandler();
        var handler2 = new TestEventHandler();

        _eventBusService.Subscribe(handler1);
        _eventBusService.Subscribe(handler2);

        // Act
        var stats = _eventBusService.GetStats();

        // Assert
        Assert.That(stats.TotalAliveHandlers, Is.EqualTo(2));
        Assert.That(stats.HandlerCounts.ContainsKey("TestEvent"), Is.True);
    }
}
