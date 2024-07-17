using MongoDB.Bson;

namespace Query.Domain.Abstractions;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; } // Id nay Mongo create automaticly

    public Guid DocumentId { get; set; } // Id cua SourceMessage: ProductID, CustomerID, OrderID

    public DateTimeOffset CreatedOnUtc => Id.CreationTime;

    public DateTimeOffset? ModifiedOnUtc { get; set; }
}