using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TheiaData.Data.Base
{
    public abstract class BaseSortableEntity : BaseEntity
    {
        [Display(Name = "Sıralama")]
        public int SortOrder { get; set; }
    }
}
