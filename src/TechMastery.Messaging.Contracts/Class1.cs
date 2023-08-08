namespace TechMastery.Messaging.Contracts;

public class ProductAdded : IMessage
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
}

public class OrderPlaced : IMessage
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }

}
