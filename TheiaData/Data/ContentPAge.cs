using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class ContentPage : BaseSortableEntity
    {
        [Display(Name = "Sayfa Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        [Display(Name = "İçerik Metni")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Text { get; set; }
        
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<ContentPage>(entity =>
            {
                entity
                .Property(p => p.Text)
                .IsRequired(); 
                entity
                 .Property(p => p.Text)
                 .IsRequired();
            });

        }
    }
}