using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public enum VariantViewMode
    {
        [Display(Name = "Metin")]
        Text,
        [Display(Name = "Görsel")]
        Picture,
        [Display(Name = "Görsel ve Metin")]
        PictureAndText
    }

    public class Variant : BaseSortableEntity
    {
        [Display(Name = "Varyant Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        [Display(Name = "Varyant Gurubu")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public int VariantGroupId { get; set; }
        public string Picture { get; set; }
        [Display(Name = "Buton Görünümü")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public VariantViewMode ViewMode { get; set; } = VariantViewMode.PictureAndText;
        [NotMapped]
        [Display(Name = "Görsel")]
        public IFormFile PictureFile { get; set; }
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