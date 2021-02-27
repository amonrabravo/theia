using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class Category : BaseSortableEntity
    {
        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        
        public int? ParentId { get; set; }
        
        public string Picture { get; set; }
        
        [NotMapped]
        [Display(Name = "Görsel")]
        public IFormFile PictureFile { get; set; }
        
        public virtual Category Parent { get; set; }
        
        public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();
        
        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; } = new HashSet<CategoryProduct>();
        
        public virtual ICollection<CategoryBanner> CategoryBanners { get; set; } = new HashSet<CategoryBanner>();
        
        public virtual ICollection<CategoryVariantGroup> CategoryVariantGroups { get; set; } = new HashSet<CategoryVariantGroup>();

        [NotMapped]
        [Display(Name = "Varyant Grupları")]
        public IEnumerable<int> SelectedVariantGroupIds { get; set; } = new List<int>();

        public string PathName => string.Join(" / ", GetPathItems().Select(p => p.Name));
        
        public IEnumerable<Category> GetPathItems()
        {
            var itemList = new List<Category>();
            itemList.Add(this);
            getParentItems(this, ref itemList);
            itemList.Reverse();
            return itemList;
        }
        private void getParentItems(Category item, ref List<Category> itemList)
        {
            if (item.Parent != null)
            {
                itemList.Add(item.Parent);
                getParentItems(item.Parent, ref itemList);
            }
        }

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

                entity
                    .Property(p => p.Picture)
                    .IsUnicode(false)
                    .IsRequired(false);


            });

        }
    }
}