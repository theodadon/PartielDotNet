using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("stock")]
public sealed class StockController : ControllerBase
{
    private readonly IStockService _stock;
    public StockController(IStockService stock) => _stock = stock;

    [HttpGet]
    public IActionResult GetAll() => Ok(_stock.GetAll());

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var p = _stock.GetByLegacyId(id);
        return p is null ? NotFound() : Ok(p);
    }
}
