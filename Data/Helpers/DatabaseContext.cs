using Data.Entities;
using Data.Entities.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Data.Helpers;

public class DatabaseContext : DbContext
{
    public const string Schema = "todo";

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    public DbSet<Todo> Todos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        base.OnModelCreating(modelBuilder);

        TodoMapper.Apply(modelBuilder.Entity<Todo>());
    }
}