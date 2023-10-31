using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Entities.Mappers;

public static class TodoMapper
{
    public static void Apply(EntityTypeBuilder<Todo> entityBuilder)
    {
        entityBuilder.ToTable("Todo");

        entityBuilder.HasKey(x => x.Id);
    }
}