using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using Theia.Data;
using Theia.Data.Base;

namespace Theia
{
    public class AppDbContext : IdentityDbContext<User, Role, int>
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(p => !p.IsAbstract && p.GetInterfaces().Any(q => q.Name == nameof(IBaseEntity)))
                .ToList()
                .ForEach(p => p.GetMethod("Build").Invoke(Activator.CreateInstance(p), new[] { builder }));

            base.OnModelCreating(builder);
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<CategoryProduct> CategoryProducts { get; set; }
        public virtual DbSet<VariantGroup> VariantGroups { get; set; }
        public virtual DbSet<Variant> Variants { get; set; }
        public virtual DbSet<CategoryVariantGroup> CategoryVariantGroups { get; set; }
        public virtual DbSet<ProductVariant> ProductVariants { get; set; }
        public virtual DbSet<ProductPicture> ProductPictures { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }



    }
}
