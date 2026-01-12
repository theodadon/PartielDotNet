using System.Collections.Concurrent;
using Ecommerce.Api.Models;

namespace Ecommerce.Api.Services;

public sealed class StockService : IStockService
{
    private readonly ConcurrentDictionary<int, Product> _productsById;
    private readonly ConcurrentDictionary<string, object> _idLocks = new();

    public StockService()
    {
        var initial = new[]
        {
            new Product { LegacyId = 1, Id = Guid.NewGuid(), Sku = "SKU-USB-C-1M", Name = "Produit A", UnitPrice = 10.00m, StockQuantity = 20 },
            new Product { LegacyId = 2, Id = Guid.NewGuid(), Sku = "SKU-MOUSE-WL", Name = "Produit B", UnitPrice = 43.10m, StockQuantity = 60 },
            new Product { LegacyId = 3, Id = Guid.NewGuid(), Sku = "SKU-KB-MECH",  Name = "Produit C", UnitPrice = 89.00m, StockQuantity = 25 },
            new Product { LegacyId = 4, Id = Guid.NewGuid(), Sku = "SKU-SSD-1TB",  Name = "Produit D", UnitPrice = 109.90m,StockQuantity = 18 },
            new Product { LegacyId = 5, Id = Guid.NewGuid(), Sku = "SKU-HEAD-BT",  Name = "Produit E", UnitPrice = 59.90m, StockQuantity = 40 },
        };

        _productsById = new ConcurrentDictionary<int, Product>(
            initial.ToDictionary(p => p.LegacyId, p => p)
        );
    }

    public IReadOnlyList<Product> GetAll()
        => _productsById.Values.OrderBy(p => p.LegacyId).ToList();

    public Product? GetByLegacyId(int id)
        => _productsById.TryGetValue(id, out var p) ? p : null;

    public bool TryDecreaseStock(int id, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        if (!_productsById.TryGetValue(id, out var product))
            return false;

        var gate = _idLocks.GetOrAdd(id.ToString(), _ => new object());

        lock (gate)
        {
            if (product.StockQuantity < quantity) return false;
            product.StockQuantity -= quantity;
            return true;
        }
    }

    public void IncreaseStock(int id, int quantity)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        if (!_productsById.TryGetValue(id, out var product))
            throw new KeyNotFoundException($"Produit introuvable pour id {id}.");

        var gate = _idLocks.GetOrAdd(id.ToString(), _ => new object());

        lock (gate)
        {
            checked { product.StockQuantity += quantity; }
        }
    }
}
