using Microsoft.EntityFrameworkCore;
using Theia.Data.Base;

namespace Theia.Data
{
    public class ProductPicture : BaseSortableEntity
    {
        public int ProductId { get; set; }
        public string Picture { get; set; }

        public virtual Product Product { get; set; }

        public override void Build(ModelBuilder builder)
        {
            builder.Entity<ProductPicture>(entity =>
            {
                entity
                .Property(p => p.Picture)
                .IsUnicode(false)
                .IsRequired();
            });
        }
    }
}
