using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Theia.Models;
using TheiaData;

namespace Theia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public AppDbContext context { get; }

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.FeaturedProducts = await context.Products.Where(p => p.Enabled).OrderBy(p => Guid.NewGuid()).Take(12).ToListAsync();
            return View();
        }
        public IActionResult Search(SearchViewModel searchViewModel)
        {
            var model = context
                .Products
                .AsEnumerable()
                .Where(p =>
                    (p.CategoryProducts.Any(q => q.Category.GetPathItems().Any(r => r.Id == searchViewModel.CategoryId)) || searchViewModel.CategoryId == null)
                    &&
                    (
                        searchViewModel.Keywords.Any(q => p.Name.ToLower().Contains(q))
                        ||
                        searchViewModel.Keywords.Any(q => p.Descriptions?.ToLower().Contains(q) ?? false)
                        ||
                        searchViewModel.Keywords.Any(q => p.ProductCode?.ToLower().Contains(q) ?? false)
                    )
                ).ToList();
            return View(model);
        }



        [Route("/Home/Error/{code:int}")]
        public IActionResult Error(int code)
        {
            return View($"~/Views/Shared/Error/{code}.cshtml");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
