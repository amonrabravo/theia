using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class VariantGroup : BaseSortableEntity
    {
        public string Name { get; set; }
        public virtual ICollection<CategoryVariantGroup> CategoryVariantGroups { get; set; } = new HashSet<CategoryVariantGroup>();
        public virtual ICollection<Variant> Variants { get; set; } = new HashSet<Variant>();
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<VariantGroup>(entity =>
            {
                entity
                .HasIndex(p => p.Name)
                .IsUnique();

                entity
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();

                entity
                .HasMany(p => p.Variants)
                .WithOne(p => p.VariantGroup)
                .HasForeignKey(p => p.VariantGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}
