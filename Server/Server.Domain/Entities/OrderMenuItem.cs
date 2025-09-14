namespace Server.Domain.Entities;

public class OrderMenuItem
{
    public Guid OrderId { get; set; }
    public Order Order { get; set; }
    public string MenuItemId { get; set; }
    public MenuItem MenuItem { get; set; }
}