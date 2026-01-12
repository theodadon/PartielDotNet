using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("products")]
public sealed class ProductsController : ControllerBase
{
    private readonly IStockService _stock;

    public ProductsController(IStockService stock)
    {
        _stock = stock;
    }

    [HttpGet]
    public ActionResult<IEnumerable<LegacyProductDto>> GetProducts()
    {
        var products = _stock.GetAll()
            .Select((p, index) => new LegacyProductDto
            {
                Id = p.LegacyId,               
                Name = p.Name,
                Price = p.UnitPrice,
                Stock = p.StockQuantity
            })
            .ToList();

        return Ok(products);
    }
}
