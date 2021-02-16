using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TheiaData;
using TheiaData.Data;
using Theia.Areas.Admin.Utils.DataTables;
using System.Collections.Generic;
using Theia.Areas.Admin.Models;

namespace Theia.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "ProductAdministrators")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        private readonly string entityName = "Ürün";

        public ProductsController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("GetList")]
        public async Task<IActionResult> GetList(Parameters parameters)
        {
            int filteredCount = 0;
            int count = 0;

            var products =
                await context
                .Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.User)
                .Where(p => (parameters.Search.Value == null || (p.Name != null && p.Name.ToLower().Contains(parameters.Search.Value.ToLower()))))
                .Select(p => new ProductListModel { 
                    Id = p.Id,
                    Name = p.Name, 
                    Picture = p.Picture,
                    Date = p.Date.ToShortDateString(),
                    Enabled = p.Enabled, 
                    Price = p.Price.ToString("c2"),
                    ProductCode = p.ProductCode,
                    UserName = p.User.Name, 
                    BrandName = p.Brand.Name, 
                    UserId = p.UserId, 
                    BrandId = p.BrandId,
                    Reviews = p.Reviews.ToString("n0")
                })
                .ToListAsync();

            filteredCount = products.Skip(parameters.Start).Count();

            count = context.Products.Count();

            Result<ProductListModel> result = new Result<ProductListModel>
            {
                draw = parameters.Draw,
                data = products,
                recordsFiltered = filteredCount,
                recordsTotal = count
            };
            return Json(result);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Brands"] = new SelectList((await context.Brands.Select(p => new { p.Id, p.Name }).ToListAsync()), "Id", "Name");
            return View(new Product { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product model)
        {
            if (ModelState.IsValid)
            {
                if (model.PictureFile != null)
                {
                    try
                    {
                        using (var image = await Image.LoadAsync(model.PictureFile.OpenReadStream()))
                        {
                            image.Mutate(p => p.Resize(new ResizeOptions
                            {
                                Size = new Size(800, 800)
                            }));
                            model.Picture = image.ToBase64String(PngFormat.Instance);
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        TempData["error"] = "Yüklenen görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        return View(model);
                    }
                }
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.Price = decimal.Parse(model.PriceText, CultureInfo.CreateSpecificCulture("tr-TR"));

                context.Entry(model).State = EntityState.Added;
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            else
                return View(model);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            var model = await context.Products.FindAsync(id);
            model.PriceText = model.Price.ToString("#.00");
            ViewData["Brands"] = new SelectList((await context.Brands.Select(p => new { p.Id, p.Name }).ToListAsync()), "Id", "Name");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                if (model.PictureFile != null)
                {
                    try
                    {
                        using (var image = await Image.LoadAsync(model.PictureFile.OpenReadStream()))
                        {
                            image.Mutate(p => p.Resize(new ResizeOptions
                            {
                                Size = new Size(800, 800)
                            }));
                            model.Picture = image.ToBase64String(PngFormat.Instance);
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        TempData["error"] = "Yüklenen görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        return View(model);
                    }
                }
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index");
            }
            else
                return View(model);
        }
        public async Task<IActionResult> Remove(int id)
        {
            var model = await context.Products.FindAsync(id);
            context.Entry(model).State = EntityState.Deleted;
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır.";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = $"{model.Name} ile ilişkili bir ya da daha fazla kayıt olduğu için silme işlemi tamamlanamıyor.";
            }
            return RedirectToAction("Index");
        }

    }
}
