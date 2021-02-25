using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TheiaData;

namespace Theia.Components
{
    public class CarouselViewComponent : ViewComponent
    {
        private readonly AppDbContext context;

        public CarouselViewComponent(AppDbContext context)
        {
            this.context = context;
        }

        public IViewComponentResult Invoke()
        {
            var model = context.Banners.Where(p => p.Enabled && (p.DateStart <= DateTime.Today || p.DateStart == null) && (p.DateEnd >= DateTime.Today || p.DateEnd == null)).ToList();
            return View(model);
        }
    }
}
