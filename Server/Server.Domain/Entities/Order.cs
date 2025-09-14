namespace Server.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }

    public ICollection<OrderMenuItem> OrderMenuItems { get; set; }
}