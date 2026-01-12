namespace Ecommerce.Api.Models;

public sealed class Product
{
    public int LegacyId { get; init; }
    public Guid Id { get; init; }
    public string Sku { get; init; } = default!;
    public string Name { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public int StockQuantity { get; set; }
}
