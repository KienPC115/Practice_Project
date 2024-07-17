using DistributedSystem.Contract.Abstractions.Message;
using MassTransit;
using MediatR;
using Query.Domain.Abstractions.Repositories;
using Query.Domain.Entities;

namespace Query.Infrastructure.Abstractions;

/*
    Vì các class kế thừa abstract class Consumer<> -> những thằng con đều recieve message from here
    -> ta set up Centralize Idempotant ở đây để lúc nào message tới cũng check được và send message đó cho thằng hanlder -> 1 công đôi việc
 */
public abstract class Consumer<TMessage> : IConsumer<TMessage>
    where TMessage : class, IDomainEvent
{
    private readonly ISender Sender;
    private readonly IMongoRepository<EventProjection> _eventRepository;
    protected Consumer(ISender sender, IMongoRepository<EventProjection> eventRepository)
    {
        Sender = sender;
        _eventRepository = eventRepository;
    }
    public async Task Consume(ConsumeContext<TMessage> context)
    {
        // Find by EventId
        // => If Existed => Ignore
        // => Not Existed => Create EventProjection

        var eventProjection = await _eventRepository.FindOneAsync(e => e.EventId == context.Message.IdEvent);

        if (eventProjection is null)
        {
            await Sender.Send(context.Message); // 

            eventProjection = new EventProjection()
            {
                EventId = context.Message.IdEvent,
                Name = context.Message.GetType().Name,
                Type = context.Message.GetType().Name
            };

            await _eventRepository.InsertOneAsync(eventProjection);
        }

    }
}