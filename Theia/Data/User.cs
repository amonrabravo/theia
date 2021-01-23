using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Theia.Data.Base;

namespace Theia.Data
{
    public class User : IdentityUser<int>, IBaseEntity
    {
        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; } = new HashSet<Category>();
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();

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

            });
        }
    }
}