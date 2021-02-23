using Microsoft.EntityFrameworkCore;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public class ContentPage : BaseSortableEntity
    {
        public string Name { get; set; }
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