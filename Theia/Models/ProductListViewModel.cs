using System.Collections.Generic;

namespace Theia.Models
{
    public class ProductListRequestViewModel
    {
        public int Page { get; set; } = 1;
        public int Count { get; set; } = 12;
        public string FilterColumn { get; set; } = "Name";
        public string OrderColumn { get; set; } = "Name";
        public OrderDirection OrderDirection { get; set; } = OrderDirection.Asc;
    }
    public class ProductListResponseViewModel<T> where T : class
    {
        public int TotalRecords { get; set; } = 0;
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }

    public enum OrderDirection
    {
        Asc, Desc
    }
}
