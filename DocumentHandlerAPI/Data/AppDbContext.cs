using DocumentHandlerAPI.Extensions;
using DocumentHandlerAPI.Interceptor;
using DocumentHandlerAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DocumentHandlerAPI.Data
{

    public class AppDbContext : DbContext
    {
        private readonly AuditInterceptor _auditInterceptor;
        public AppDbContext(DbContextOptions options, AuditInterceptor auditInterceptor) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            _auditInterceptor = auditInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(_auditInterceptor);
            //optionsBuilder.AddInterceptors(_softDeleteInterceptor);
        }

        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                // Check if the entity has an IsDeleted property
                var isDeletedProperty = entityType.FindProperty("IsDeleted");
                if (isDeletedProperty != null && isDeletedProperty.ClrType == typeof(bool))
                {
                    // Get the entity type
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    // Create expression: e => e.IsDeleted == false
                    var filter = Expression.Lambda(
                        Expression.Equal(
                            Expression.Property(parameter, "IsDeleted"),
                            Expression.Constant(false)
                        ),
                        parameter
                    );

                    // Apply filter to entity
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }

            modelBuilder.Entity<Document>().ApplyAuditedEntityConfiguration(d => d.Id);
        }

    }
}
