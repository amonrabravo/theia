using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Theia.Areas.Admin.Utils;
using Theia.Data.Base;

namespace Theia.Data
{
    public class Category : BaseSortableEntity
    {
        [Display(Name = "Kategori Adı")]
        [Required(ErrorMessage = "{0} alanı boş bırakılamaz!")]
        public string Name { get; set; }
        public int? ParentId { get; set; }

        public virtual Category Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; } = new HashSet<Category>();
        public virtual ICollection<CategoryProduct> CategoryProducts { get; set; } = new HashSet<CategoryProduct>();
        public virtual ICollection<CategoryVariantGroup> CategoryVariantGroups { get; set; } = new HashSet<CategoryVariantGroup>();

        public static string GetPath(AppDbContext context, int id)
        {
            var nameList = new List<string>();
            getParents(context, id,ref nameList);
            nameList.Reverse();
            nameList.Insert(0,"Kategoriler");
            return string.Join(" / ", nameList);
        }



        private static void getParents(AppDbContext context, int id, ref List<string> nameList)
        {
            var category = context.Categories.Include(p => p.Parent).Single(p => p.Id == id);
            nameList.Add(category.Name);
            if (category.Parent != null)
                getParents(context, category.ParentId.Value, ref nameList);
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

            });

        }
    }
}