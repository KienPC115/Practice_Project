using DistributedSystem.Domain.Abstractions.Aggregates;
using DistributedSystem.Domain.Abstractions.Entities;
using static DistributedSystem.Domain.Exceptions.ProductException;

namespace DistributedSystem.Domain.Entities;

public class Product : AggregateRoot<Guid>, IAuditableEntity
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public string Description { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? ModifiedOnUtc { get; set; }

    public static Product CreateProduct(Guid id, string name, decimal price, string description)
    {
        if (name.Length > 50) // giả sử có cái nghiệp vụ như z 
            throw new ProductFieldException(nameof(Name));

        var product = new Product(id, name, price, description);

        product.RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductCreated(Guid.NewGuid(), product.Id,
            product.Name, product.Price,
            product.Description
            ));

        return product;
    }

    public Product(Guid id, string name, decimal price, string description)
    {
        Id = id;
        Name = name;
        Price = price;
        Description = description;
    }

    // tại sao ko để thằng này là static -> vì khi Update hoặc Delete thì nó phải cần tham chiếu đến 1 đối tượng cụ thể -> chính thằng đó mới là thằng
    //update, delete -> nên ko để static được 
    public void Update(string name, decimal price, string description)
    {
        if (name.Length > 50)
            throw new ProductFieldException(nameof(Name));

        Name = name;
        Price = price;
        Description = description;

        RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductUpdated(Guid.NewGuid(), Id, name, price, description));
    }

    public void Delete()
        => RaiseDomainEvent(new Contract.Services.V1.Product.DomainEvent.ProductDeleted(Guid.NewGuid(), Id));

    // Sau này có liên quan gì tới thằng Product nữa vd: checkout về 1 cái code, kiểm tra gì tồn tại hay không?
    //-> Tất cả liên quan tới nghiệp vụ product này thôi thì Code ở đây (HERE) -> chỉ lq tới Product thôi 
    //=> code như vậy nó Centralize tập trung hết ở đây. Mấy chỗ khác chỉ cần gọi ra và dùng ko thay đổi đc business CreateProduct, UpdatePruct, ..
    //nếu sai thì chỉ cần vô đúng chỗ này sửa thôi 
    //-> còn những cái lq tới nghiệp vụ lấy ra sản phẩm - ko liên quan tới nghiệp vụ của Domain -> thì vẫn nằm ở ngoài Application 
}
