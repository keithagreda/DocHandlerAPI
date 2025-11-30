using DocumentHandlerAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Linq.Expressions;

namespace DocumentHandlerAPI.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyAuditedEntityConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, Expression<Func<TEntity, Ulid>> propertyExpression)
            where TEntity : AuditedEntity
        {
            builder.Property(e => e.CreationTime).HasColumnType("timestamptz");
            builder.Property(e => e.LastModificationTime).HasColumnType("timestamptz");
            builder.Property(e => e.DeletionTime).HasColumnType("timestamptz");
            builder.Property(propertyExpression)
                .HasConversion(
                    ulid => ulid.ToString(),
                    str => Ulid.Parse(str)
                )
                .HasMaxLength(26); // ULID string length is always 26 characters
        }

        // If you want to configure all ULID properties globally
        public static void ConfigureUlidProperties(this ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(Ulid))
                    {
                        property.SetValueConverter(
                            new ValueConverter<Ulid, string>(
                                ulid => ulid.ToString(),
                                str => Ulid.Parse(str)
                            )
                        );
                        property.SetMaxLength(26);
                    }
                }
            }
        }
    }
}
