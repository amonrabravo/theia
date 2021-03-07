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

        public IActionResult Index()
        {
            ViewBag.FeaturedProducts = context.Products.AsNoTracking().Where(p => p.Enabled).OrderBy(p => Guid.NewGuid()).Take(12);
            return View();
        }

        public IActionResult Search(SearchViewModel searchViewModel)
        {
            ViewBag.Results = context
                .Products
                .AsNoTracking()
                //.Where(p =>
                //    (p.CategoryProducts.Any(q => q.Category.GetPathItems().Any(r => r.Id == searchViewModel.CategoryId)) || searchViewModel.CategoryId == null)
                //    &&
                //    (
                //        searchViewModel.Keywords.Any(q => EF.Functions.Contains("Name", q))
                //        ||
                //        searchViewModel.Keywords.Any(q => EF.Functions.Contains("Descriptions", q))
                //        ||
                //        searchViewModel.Keywords.Any(q => EF.Functions.Contains("ProductCode", q))

                //    )
                //)
                .AsQueryable();
            return View();
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
