using Microsoft.EntityFrameworkCore;
using TheiaData.Data.Base;

namespace TheiaData.Data
{
    public enum CommentType
    {
        Positive, Negative
    }

    public class Comment : BaseEntity
    {
        public int ProductId { get; set; }
        public CommentType CommentType { get; set; } = CommentType.Positive;
        public string Text { get; set; }
        public virtual Product Product { get; set; }
        public override void Build(ModelBuilder builder)
        {
            builder.Entity<Comment>(entity =>
            {
                entity
                .Property(p => p.Text)
                .IsRequired();
            });
        }
    }
}
