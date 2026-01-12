using Ecommerce.Api.Models;
using Ecommerce.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("orders")] // ✅ /orders (pas /api/orders)
public sealed class OrdersController : ControllerBase
{
    private readonly IStockService _stock;

    // verrou global pour rendre la commande "tout ou rien" (transaction in-memory)
    private static readonly object _orderGate = new();

    public OrdersController(IStockService stock) => _stock = stock;

    [HttpPost]
    public ActionResult<CreateOrderResponse> Create([FromBody] CreateOrderRequest request)
    {
        var errors = new List<string>();

        if (request.Products is null || request.Products.Count == 0)
            errors.Add("La commande doit contenir au moins un produit.");

        // validation basique
        foreach (var line in request.Products)
        {
            if (line.Id <= 0) errors.Add("Un id produit est invalide.");
            if (line.Quantity <= 0) errors.Add($"La quantité pour le produit id {line.Id} doit être > 0.");
        }

        if (errors.Count > 0)
            return BadRequest(new ErrorResponse { Errors = errors });

        // Regroupement: si même id plusieurs fois, on somme (robuste)
        var grouped = request.Products
            .GroupBy(p => p.Id)
            .Select(g => new { Id = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToList();

        List<(int Id, int Quantity, string Name, decimal UnitPrice)> resolved = new();

        // ✅ Transaction in-memory: on vérifie tout, puis on décrémente tout sous un lock
        lock (_orderGate)
        {
            // 1) Vérifier existence + stock suffisant
            foreach (var g in grouped)
            {
                var product = _stock.GetByLegacyId(g.Id);
                if (product is null)
                {
                    errors.Add($"Le produit avec l'identifiant {g.Id} n'existe pas");
                    continue;
                }

                if (product.StockQuantity < g.Quantity)
                {
                    errors.Add($"il ne reste que {product.StockQuantity} exemplaires du produit {product.Name}");
                    continue;
                }

                resolved.Add((g.Id, g.Quantity, product.Name, product.UnitPrice));
            }

            if (errors.Count > 0)
                return BadRequest(new ErrorResponse { Errors = errors });

            // 2) Appliquer la décrémentation (ne devrait pas échouer vu qu’on est sous lock)
            foreach (var r in resolved)
            {
                var ok = _stock.TryDecreaseStock(r.Id, r.Quantity);
                if (!ok)
                {
                    // extrêmement rare ici, mais on sécurise
                    var current = _stock.GetByLegacyId(r.Id)!.StockQuantity;
                    errors.Add($"il ne reste que {current} exemplaires du produit {r.Name}");
                }
            }

            if (errors.Count > 0)
            {
                // rollback
                foreach (var r in resolved)
                    _stock.IncreaseStock(r.Id, r.Quantity);

                return BadRequest(new ErrorResponse { Errors = errors });
            }
        }

 // 3) Construire la réponse + totaux (avec règle 1)
var lines = resolved.Select(r =>
{
    var baseLineTotal = r.UnitPrice * r.Quantity;

    // Règle 1: si quantité > 5 => -10% sur ce produit
    var lineTotal = (r.Quantity > 5)
        ? Math.Round(baseLineTotal * 0.90m, 2, MidpointRounding.AwayFromZero)
        : baseLineTotal;

    return new OrderLineDto
    {
        Id = r.Id,
        Name = r.Name,
        Quantity = r.Quantity,
        PricePerUnit = r.UnitPrice, // prix unitaire affiché (non remisé)
        Total = lineTotal           // total de ligne après éventuelle remise 10%
    };
}).ToList();

// Subtotal après règle 1 (remises par produit)
var subtotal = lines.Sum(l => l.Total);

// ---------- Code promo (règles 2.c) ----------
decimal promoPercent = 0m;

if (!string.IsNullOrWhiteSpace(request.PromoCode))
{
    if (subtotal < 50m)
        return BadRequest(new ErrorResponse { Errors = new List<string> { "Les codes promo sont valables qu'à partir de 50 euros d'achat" } });

    if (string.Equals(request.PromoCode, "DISCOUNT20", StringComparison.OrdinalIgnoreCase))
        promoPercent = 20m;
    else if (string.Equals(request.PromoCode, "DISCOUNT10", StringComparison.OrdinalIgnoreCase))
        promoPercent = 10m;
    else
        return BadRequest(new ErrorResponse { Errors = new List<string> { "Le code promo est invalide" } });
}


// Discounts (règle 2 + promo)
var discounts = new List<DiscountDto>();

// Règle 2: si total commande > 100 => -5% sur le total commande (type "order")
decimal totalPercent = promoPercent;

// Règle 2.b: 5% si total > 100
if (subtotal > 100m)
    totalPercent += 5m;

// Si aucune remise, pas d'entrée discounts
if (totalPercent > 0)
{
    var totalDiscount = Math.Round(subtotal * (totalPercent / 100m), 2, MidpointRounding.AwayFromZero);
    discounts.Add(new DiscountDto
    {
        Type = "order",
        Value = totalDiscount
    });
}

var total = subtotal - discounts.Sum(d => d.Value);
if (total < 0) total = 0;

var response = new CreateOrderResponse
{
    Products = lines,
    Discounts = discounts,
    Total = total
};

return Ok(response);
    }
}