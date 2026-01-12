namespace Ecommerce.Api.Models;

public sealed class DbPromoCode
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public decimal Percent { get; set; }
}
