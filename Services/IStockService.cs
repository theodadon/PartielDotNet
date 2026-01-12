using Ecommerce.Api.Models;

namespace Ecommerce.Api.Services;

public interface IStockService
{
    IReadOnlyList<Product> GetAll();
    Product? GetByLegacyId(int id);

    bool TryDecreaseStock(int id, int quantity);
    void IncreaseStock(int id, int quantity);
}
