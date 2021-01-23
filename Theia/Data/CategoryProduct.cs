using Microsoft.EntityFrameworkCore;
using Theia.Data.Base;

namespace Theia.Data
{
    public class CategoryProduct : IBaseEntity
    {
        public int CategoryId { get; set; }
        public int ProductId { get; set; }

        public virtual Category Category { get; set; }
        public virtual Product Product { get; set; }

        public void Build(ModelBuilder builder)
        {
            builder.Entity<CategoryProduct>(entity => {
                
                entity
                .HasKey(p => new { p.CategoryId, p.ProductId });

                entity
                .HasOne(p => p.Category)
                .WithMany(p => p.CategoryProducts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasOne(p => p.Product)
                .WithMany(p => p.CategoryProducts)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}