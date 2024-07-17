using DistributedSystem.Contract.Abstractions.Message;
using DistributedSystem.Domain.Abstractions.Entities;

namespace DistributedSystem.Domain.Abstractions.Aggregates;

public abstract class AggregateRoot<T> : Entity<T>
{
    /* Domain Driven Design: Aggreate - là 1 nhóm các Entity - Entity là 1 nhóm đối tượng có định danh (Order - Product - Customer).
    - Tất cả thao tác ta làm 1 trên 1 cụm -> chia để trị
    - Thế nhưng khi tổng quát lên thì có nhiều cái người ta muốn chia nhỏ thành object
    Vd: Account {
            Id,
            Address(City, street) -> không có Id - định danh vì địa chỉ thì có 1 thôi -> ValueObject
            Address2
            profile() -> object trong object rất là nhiều
        }
     
    */
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);
}