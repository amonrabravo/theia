using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class Brand : BaseSortableEntity
    {
        [Display(Name = "Marka Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        public string Picture { get; set; }
        [NotMapped]
        [Display(Name = "Görsel")]
        public IFormFile PictureFile { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Brand>(entity =>
            {
                entity
                    .HasIndex(p => new { p.Name })
                    .IsUnique();

                entity
                    .Property(p => p.Name)
                    .HasMaxLength(250)
                    .IsRequired();

                entity
                    .Property(p => p.Picture)
                    .IsUnicode(false);
                entity
                    .HasMany(p => p.Products)
                    .WithOne(p => p.Brand)
                    .HasForeignKey(p => p.BrandId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

        }
    }
}