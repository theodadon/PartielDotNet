using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Services;

public sealed class EfStockService : IStockService
{
    private readonly EcommerceDbContext _db;

    public EfStockService(EcommerceDbContext db) => _db = db;

    public IReadOnlyList<Product> GetAll()
        => _db.Products.AsNoTracking()
            .OrderBy(p => p.Id)
            .Select(p => new Product
            {
                LegacyId = p.Id,
                Id = Guid.Empty, 
                Sku = $"SKU-{p.Id}",
                Name = p.Name,
                UnitPrice = p.UnitPrice,
                StockQuantity = p.StockQuantity
            })
            .ToList();

    public Product? GetByLegacyId(int id)
    {
        var p = _db.Products.AsNoTracking().FirstOrDefault(x => x.Id == id);
        if (p is null) return null;

        return new Product
        {
            LegacyId = p.Id,
            Id = Guid.Empty,
            Sku = $"SKU-{p.Id}",
            Name = p.Name,
            UnitPrice = p.UnitPrice,
            StockQuantity = p.StockQuantity
        };
    }

    public bool TryDecreaseStock(int id, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var p = _db.Products.FirstOrDefault(x => x.Id == id);
        if (p is null) return false;

        if (p.StockQuantity < quantity) return false;

        p.StockQuantity -= quantity;
        _db.SaveChanges();
        return true;
    }

    public void IncreaseStock(int id, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var p = _db.Products.FirstOrDefault(x => x.Id == id)
            ?? throw new KeyNotFoundException($"Produit introuvable pour id {id}.");

        checked { p.StockQuantity += quantity; }
        _db.SaveChanges();
    }
}
