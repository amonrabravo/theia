using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theia.Data.Base
{
    public abstract class BaseSortableEntity : BaseEntity
    {
        public int SortOrder { get; set; }
    }
}
