using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class Banner : BaseSortableEntity
    {
        public string Picture { get; set; }

        [Display(Name = "İlk Tarih")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateStart { get; set; }

        [Display(Name = "Son Tarih")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateEnd { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }

        [NotMapped]
        [Display(Name = "Görsel")]
        public IFormFile PictureFile { get; set; }
        
        [NotMapped]
        [Display(Name = "Kategoriler")]
        public IEnumerable<int> SelectedCategoryIds { get; set; } = new List<int>();

        public virtual ICollection<CategoryBanner> CategoryBanners { get; set; } = new HashSet<CategoryBanner>();

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Brand>(entity =>
            {
                entity
                .Property(p => p.Picture)
                .IsRequired()
                .IsUnicode(false);

            });

        }
    }
}