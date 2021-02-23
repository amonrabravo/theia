using Microsoft.EntityFrameworkCore;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class CategoryBanner : IBaseEntity
    {
        public int CategoryId { get; set; }
        public int BannerId { get; set; }
        public virtual Category Category { get; set; }
        public virtual Banner Banner { get; set; }
        public void Build(ModelBuilder builder)
        {
            builder.Entity<CategoryBanner>(entity => {
                
                entity
                .HasKey(p => new { p.CategoryId, p.BannerId });

                entity
                .HasOne(p => p.Category)
                .WithMany(p => p.CategoryBanners)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

                entity
                .HasOne(p => p.Banner)
                .WithMany(p => p.CategoryBanners)
                .HasForeignKey(p => p.BannerId)
                .OnDelete(DeleteBehavior.Cascade);

            });
        }
    }
}