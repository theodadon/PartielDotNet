namespace Ecommerce.Api.Models;

public sealed class DbProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
}
