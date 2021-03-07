using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheiaData.Data;

namespace Theia.Models
{
    public class ProductListViewModel
    {
        public IList<Product>  Products { get; set; }
        public IList<ProductListPage> Pages { get; set; } = new List<ProductListPage>();
        public int PageSize { get; set; } = 12;
        public int TotalCount { get; set; }
        public int AbsolutePage { get; set; } = 1;
        public string OrderColumn { get; set; } = nameof(Product.Name);
        public ProductListOrderDirection OrderDirection { get; set; } = ProductListOrderDirection.Asc;

    }

    public class ProductListPage
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }

    public enum ProductListOrderDirection
    {
        Asc, Desc
    }
}
