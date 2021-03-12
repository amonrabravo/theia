using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TheiaData.Data;

namespace Theia.Components
{
    public class ProductListViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProductListViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke(IQueryable<Product> products)
        {
            return View(products);
        }
    }
}
