using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class Product : BaseEntity
    {
        [Display(Name = "Ürün Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        [Display(Name = "Ürün Kodu")]
        public string ProductCode { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }
        [Display(Name = "Açıklamalar")]
        public string Descriptions { get; set; }
        [Display(Name = "Görüntülenme")]
        public int Reviews { get; set; } = 0;
        [Display(Name = "Marka")]
        public int? BrandId { get; set; }
        [NotMapped]
        [Display(Name = "Görsel")]
        public IFormFile PictureFile { get; set; }
        [NotMapped]
        [Display(Name = "Fiyat")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        [RegularExpression(@"^([0-9]+(\,[0-9]{1,2})?)$", ErrorMessage = "Lütfen geçerli bir fiyat yazınız")]
        public string PriceText { get; set; }
        [NotMapped]
        [Display(Name = "Kategoriler")]
        public IEnumerable<int> SelectedCategoryIds { get; set; } 
        public virtual Brand Brand { get; set; }
        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; } = new HashSet<CategoryProduct>();
        public virtual ICollection<ProductVariant> ProductVariants { get; set; } = new HashSet<ProductVariant>();
        public virtual ICollection<ProductPicture> ProductPictures { get; set; } = new HashSet<ProductPicture>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Product>(entity =>
            {
                entity
                    .HasIndex(p => new { p.Name })
                    .IsUnique();

                entity
                    .HasIndex(p => new { p.ProductCode })
                    .IsUnique();

                entity
                    .Property(p => p.Name)
                    .HasMaxLength(250)
                    .IsRequired();

                entity
                    .Property(p => p.ProductCode)
                    .HasMaxLength(250);

                entity
                    .Property(p => p.Price)
                    .HasPrecision(18, 4);

                entity
                    .Property(p => p.Picture)
                    .IsUnicode(false);

                entity
                    .HasMany(p => p.ProductPictures)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity
                    .HasMany(p => p.Comments)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

        }
    }
}