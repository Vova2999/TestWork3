#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Client.Domain.Dtos;

public class OrderMenuItemDto
{
    public string Id { get; set; }
    public decimal Quantity { get; set; }
}