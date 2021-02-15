using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class Variant : BaseEntity
    {
        public string Name { get; set; }
        public int VariantGroupId { get; set; }
        public string Image { get; set; }
        public bool UseImage { get; set; } = false;
        public virtual VariantGroup VariantGroup { get; set; }
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Variant>(entity =>
            {
                entity
                    .HasIndex(p => new { p.Name, p.VariantGroupId })
                    .IsUnique();

                entity
                    .Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();

            });

        }
    }
}