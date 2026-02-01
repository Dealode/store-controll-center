namespace BusinessControl.Domain.Aggregates;

public class AggregateRoot
{
    public Guid Id { get; protected set; }
    public int Version { get; protected set; }
    
    private readonly List<object> _events = new();
    
    public IEnumerable<object> GetUncommittedEvents() => _events;
    
    public void ClearEvents() => _events.Clear();
    
    protected void ApplyEvent(object @event)
    {
        _events.Add(@event);
        ((dynamic)this).Apply((dynamic)@event);
    }
}