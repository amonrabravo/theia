using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Theia.Data.Base
{
    public interface IBaseEntity
    {
        void Build(ModelBuilder builder);
    }

    public abstract class BaseEntity : IBaseEntity 
    {
        protected BaseEntity()
        {
        }

        public virtual int Id { get; set; }
        public virtual int UserId { get; set; }
        public virtual bool Enabled { get; set; }
        public virtual DateTime Date { get; set; }

        public virtual User User { get; set; }

        public abstract void Build(ModelBuilder builder);

    }
}