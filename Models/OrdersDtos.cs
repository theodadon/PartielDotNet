using System.Text.Json.Serialization;

namespace Ecommerce.Api.Models;

public sealed class CreateOrderRequest
{
    [JsonPropertyName("products")]
    public List<OrderProductRequest> Products { get; init; } = new();

    [JsonPropertyName("promo_code")]
    public string? PromoCode { get; init; }
}

public sealed class OrderProductRequest
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }
}

public sealed class CreateOrderResponse
{
    [JsonPropertyName("products")]
    public List<OrderLineDto> Products { get; init; } = new();

    [JsonPropertyName("discounts")]
    public List<DiscountDto> Discounts { get; init; } = new();

    [JsonPropertyName("total")]
    public decimal Total { get; init; }
}

public sealed class OrderLineDto
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [JsonPropertyName("price_per_punit")]
    public decimal PricePerUnit { get; init; }

    [JsonPropertyName("total")]
    public decimal Total { get; init; }
}

public sealed class DiscountDto
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = default!;

    [JsonPropertyName("value")]
    public decimal Value { get; init; }
}

public sealed class ErrorResponse
{
    [JsonPropertyName("errors")]
    public List<string> Errors { get; init; } = new();
}
