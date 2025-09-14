#pragma warning disable CS8618
// ReSharper disable CollectionNeverUpdated.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Server.Domain.Entities;

public class MenuItem
{
    public string Id { get; set; }
    public string Article { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool IsWeighted { get; set; }
    public string FullPath { get; set; }
    public List<string> Barcodes { get; set; }

    public ICollection<OrderMenuItem> OrderMenuItems { get; set; }
}