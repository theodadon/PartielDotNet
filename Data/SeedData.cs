using Ecommerce.Api.Models;

namespace Ecommerce.Api.Data;

public static class SeedData
{
    public static readonly DbProduct[] Products =
    {
        new DbProduct { Id = 1, Name = "Produit A", UnitPrice = 10.00m, StockQuantity = 20 },
        new DbProduct { Id = 2, Name = "Produit B", UnitPrice = 43.10m, StockQuantity = 60 },
        new DbProduct { Id = 3, Name = "Produit C", UnitPrice = 89.00m, StockQuantity = 25 },
        new DbProduct { Id = 4, Name = "Produit D", UnitPrice = 109.90m, StockQuantity = 18 },
        new DbProduct { Id = 5, Name = "Produit E", UnitPrice = 59.90m, StockQuantity = 40 }
    };

    public static readonly DbPromoCode[] PromoCodes =
    {
        new DbPromoCode { Code = "DISCOUNT10", Percent = 10m },
        new DbPromoCode { Code = "DISCOUNT20", Percent = 20m }
    };
}
