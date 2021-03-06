﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Theia.Data.Base;

namespace Theia.Data
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Picture { get; set; }
        public string Descriptions { get; set; }
        public int Reviews { get; set; } = 0;

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
                    .Property(p => p.Name)
                    .HasMaxLength(250)
                    .IsRequired();

                entity
                .Property(p => p.Price)
                .HasColumnType("money");

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