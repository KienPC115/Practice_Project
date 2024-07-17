using MassTransit;

namespace DistributedSystem.Contract.Abstractions.Message;

[ExcludeFromTopology] // ko tạo ra exchange rác nữa -> rabbitmq ko có exchange idomain-event này
public interface IDomainEvent //: INotification
{
    public Guid IdEvent { get; init; }
}