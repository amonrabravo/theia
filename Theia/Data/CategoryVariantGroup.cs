using Microsoft.EntityFrameworkCore;
using Theia.Data.Base;

namespace Theia.Data
{
    public class CategoryVariantGroup : IBaseEntity
    {
        public int CategoryId { get; set; }
        public int VariantGroupId { get; set; }

        public virtual Category Category { get; set; }
        public virtual VariantGroup VariantGroup { get; set; }

        public void Build(ModelBuilder builder)
        {
            builder.Entity<CategoryVariantGroup>(entity => {
                
                entity
                .HasKey(p => new { p.CategoryId, p.VariantGroupId });

                entity
                .HasOne(p => p.Category)
                .WithMany(p => p.CategoryVariantGroups)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasOne(p => p.VariantGroup)
                .WithMany(p => p.CategoryVariantGroups)
                .HasForeignKey(p => p.VariantGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}