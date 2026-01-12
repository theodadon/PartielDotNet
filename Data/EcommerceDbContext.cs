using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Data;

public sealed class EcommerceDbContext : DbContext
{
    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options)
        : base(options) { }

    public DbSet<DbProduct> Products => Set<DbProduct>();
    public DbSet<DbPromoCode> PromoCodes => Set<DbPromoCode>();
}
