namespace Query.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)] // sử dụng cho class thôi, không kế thừa
public class BsonCollectionAttribute : Attribute
{
    // vì entity mình đặt là ProductProject thì cái tên này nó không meaning khi xuống dưới MongoDb tạo collection
    //để nó đặt tên theo í mình thì m dùng custom cái Attribute này lấy được cái tên mình muốn ở class Entity
    public string CollectionName { get; }

    public BsonCollectionAttribute(string collectionName)
    {
        CollectionName = collectionName;
    }
}