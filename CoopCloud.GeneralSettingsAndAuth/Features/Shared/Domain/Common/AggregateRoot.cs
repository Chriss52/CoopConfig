namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Domain.Common;

public abstract class AggregateRoot<TId>
{
    public TId Id { get; protected set; } = default!;
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent eventItem) => _domainEvents.Add(eventItem);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
