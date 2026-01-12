namespace Ecommerce.Api.Models;

public sealed class LegacyProductDto
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public decimal Price { get; init; }
    public int Stock { get; init; }
}
