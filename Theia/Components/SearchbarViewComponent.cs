using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Theia.Models;
using TheiaData;

namespace Theia.Components
{
    public class SearchBarViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public SearchBarViewComponent(AppDbContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            ViewBag.Categories = context.Categories.Where(p => p.Enabled).ToList().OrderBy(p => p.PathName).ThenBy(p => p.SortOrder);
            return View(new SearchViewModel { });
        }
    }
}
