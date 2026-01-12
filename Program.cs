using Ecommerce.Api.Data;
using Ecommerce.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Lire appsettings pour savoir si on est en EF ou singleton
var stockProvider = builder.Configuration["StockProvider"] ?? "Singleton";

if (stockProvider.Equals("Ef", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<EcommerceDbContext>(o =>
        o.UseInMemoryDatabase("EcommerceDb"));

    builder.Services.AddScoped<IStockService, EfStockService>();
}
else if (stockProvider.Equals("Singleton", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IStockService, StockService>();
}
else
{
    throw new InvalidOperationException($"StockProvider inconnu: {stockProvider}");
}

Console.WriteLine($"StockProvider actif: {stockProvider}");

var app = builder.Build();

// Seed uniquement si EF
if (stockProvider.Equals("Ef", StringComparison.OrdinalIgnoreCase))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();

    if (!db.Products.Any())
    {
        db.Products.AddRange(SeedData.Products);
        db.PromoCodes.AddRange(SeedData.PromoCodes);
        db.SaveChanges();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program { }
