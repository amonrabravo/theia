using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.ComponentModel.DataAnnotations;

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

        [Display(Name = "Etkin")]
        public virtual bool Enabled { get; set; }

        [Display(Name = "Tarih")]
        public virtual DateTime Date { get; set; }

        public virtual User User { get; set; }

        public abstract void Build(ModelBuilder builder);

    }
}