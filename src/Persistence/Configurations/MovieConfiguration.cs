using Execution.Domain.Entities;
using Execution.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Configurations;

/// <summary>
/// EF Core mapping for Movie entity.
/// Enforces unique index on Title (case-insensitive via default SQL Server collation).
/// </summary>
public sealed class MovieConfiguration : IEntityTypeConfiguration<Movie>
{
    public void Configure(EntityTypeBuilder<Movie> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.HasIndex(m => m.Title)
            .IsUnique();

        builder.Property(m => m.Genre)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.ToTable("Movies");
    }
}
