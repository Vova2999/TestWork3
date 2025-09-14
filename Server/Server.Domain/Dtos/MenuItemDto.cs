#pragma warning disable CS8618
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Server.Domain.Dtos;

public class MenuItemDto
{
    public string Id { get; set; }
    public string Article { get; set; }
    public string Name { get; set; }
    public decimal? Price { get; set; }
    public bool IsWeighted { get; set; }
    public string FullPath { get; set; }
    public ICollection<string> Barcodes { get; set; }
}