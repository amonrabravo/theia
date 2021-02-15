using Microsoft.EntityFrameworkCore;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class ProductVariant : IBaseEntity
    {
        public int ProductId { get; set; }
        public int VariantId { get; set; }
        public virtual Product Product { get; set; }
        public virtual Variant Variant { get; set; }
        public void Build(ModelBuilder builder)
        {
            builder.Entity<ProductVariant>(entity => {
                
                entity
                .HasKey(p => new { p.ProductId, p.VariantId });

                entity
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasOne(p => p.Variant)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(p => p.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}