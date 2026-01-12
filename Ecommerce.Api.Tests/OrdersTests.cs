using System.Net;
using System.Net.Http.Json;
using Ecommerce.Api.Models;
using Xunit;

namespace Ecommerce.Api.Tests;

public sealed class OrdersTests
{
    private HttpClient CreateClient()
{
    var factory = new ApiFactory();
    return factory.CreateClient();
}
    private async Task<HttpResponseMessage> PostOrder(object payload)
    {
        return await CreateClient().PostAsJsonAsync("/orders", payload);
    }

    private async Task<T> Read<T>(HttpResponseMessage res)
    {
        return (await res.Content.ReadFromJsonAsync<T>())!;
    }
    [Fact]
    public async Task QuantityAbove5_Gets10PercentDiscountOnLine()
    {
        var res = await PostOrder(new
        {
            products = new[] { new { id = 1, quantity = 6 } }
        });

        var data = await Read<CreateOrderResponse>(res);

        Assert.Equal(54.00m, data.Products[0].Total);
    }

    [Fact]
    public async Task StockTooLow_ReturnsExactMessage()
    {
        var res = await PostOrder(new
        {
            products = new[] { new { id = 1, quantity = 999 } }
        });

        Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);

        var err = await Read<ErrorResponse>(res);
        Assert.Contains("il ne reste que 20 exemplaires du produit Produit A", err.Errors);
    }
    [Fact]
    public async Task PromoBelow50_IsRejected()
    {
        var res = await PostOrder(new
        {
            products = new[] { new { id = 1, quantity = 2 } },
            promo_code = "DISCOUNT10"
        });

        var err = await Read<ErrorResponse>(res);
        Assert.Contains("Les codes promo sont valables qu'à partir de 50 euros d'achat", err.Errors);
    }

    [Fact]
    public async Task InvalidPromoCode_IsRejected()
    {
        var res = await PostOrder(new
        {
            products = new[] { new { id = 4, quantity = 1 } },
            promo_code = "HACK"
        });

        var err = await Read<ErrorResponse>(res);
        Assert.Contains("Le code promo est invalide", err.Errors);
    }

    [Fact]
    public async Task PromoAndOrderDiscountsAreAdditive()
    {
        var res = await PostOrder(new
        {
            products = new[] { new { id = 4, quantity = 2 } },
            promo_code = "DISCOUNT10"
        });

        var data = await Read<CreateOrderResponse>(res);

        Assert.Equal(32.97m, data.Discounts[0].Value);
        Assert.Equal(186.83m, data.Total);
    }
    [Fact]
    public async Task UnknownProductId_ReturnsError()
    {
        var res = await PostOrder(new
        {
            products = new[] { new { id = 999, quantity = 1 } }
        });

        var err = await Read<ErrorResponse>(res);
        Assert.Contains("Le produit avec l'identifiant 999 n'existe pas", err.Errors);
    }
    [Fact]
    public async Task MultipleErrorsAreAllReturned()
    {
        var res = await PostOrder(new
        {
            products = new[]
            {
                new { id = 999, quantity = 1 },  // n'existe pas
                new { id = 1, quantity = 999 }   // stock insuffisant
            }
        });

        var err = await Read<ErrorResponse>(res);

        Assert.Equal(2, err.Errors.Count);
        Assert.Contains("Le produit avec l'identifiant 999 n'existe pas", err.Errors);
        Assert.Contains("il ne reste que 20 exemplaires du produit Produit A", err.Errors);
    }
}
