using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Theia.Data.Base;

namespace Theia.Data
{
    public class Category : BaseSortableEntity
    {
        public string Name { get; set; }
        public int? ParentId { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();
        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; } = new HashSet<CategoryProduct>();
        public virtual ICollection<CategoryVariantGroup> CategoryVariantGroups { get; set; } = new HashSet<CategoryVariantGroup>();

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Category>(entity =>
            {
                entity
                    .HasIndex(p => new { p.Name, p.ParentId })
                    .IsUnique();

                entity
                    .Property(p => p.Name)
                    .HasMaxLength(50)
                    .IsRequired();

                entity
                    .HasMany(p => p.Children)
                    .WithOne(p => p.Parent)
                    .HasForeignKey(p => p.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

        }
    }
}