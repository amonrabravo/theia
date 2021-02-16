using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theia.Areas.Admin.Models
{
    public class ProductListModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ProductCode { get; set; }
        public string Price { get; set; }
        public string Date { get; set; }
        public string UserName { get; set; }
        public string BrandName { get; set; }
        public int UserId { get; set; }
        public int? BrandId { get; set; }
        public string Picture { get; set; }
        public bool Enabled { get; set; }
        public string Reviews { get; set; }


    }
}
