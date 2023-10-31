using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Security.Helpers;

public class IdentityDatabaseContext : IdentityDbContext
{
    public const string Schema = "user";

    public IdentityDatabaseContext(DbContextOptions<IdentityDatabaseContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);
    }
}