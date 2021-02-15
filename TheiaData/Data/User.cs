using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class User : IdentityUser<int>, IBaseEntity
    {
        [Display(Name = "Kullanıcı Adı")]
        public string Name { get; set; }
        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
        public virtual ICollection<Brand> Brands { get; set; } = new HashSet<Brand>();
        public virtual ICollection<VariantGroup> VariantGroups { get; set; } = new HashSet<VariantGroup>();
        public void Build(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity
                .Property(p => p.Name)
                .HasMaxLength(50)
                .IsRequired();

                entity
                .HasMany(p => p.Categories)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity
                .HasMany(p => p.Products)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

                entity
                .HasMany(p => p.Brands)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            });
        }
    }
}