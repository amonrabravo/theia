using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.Mvc.Rendering;

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
            var query = context
                .Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.User)
                .Where(p =>
                    (parameters.Search.Value == null || (p.Name != null && p.Name.ToLower().Contains(parameters.Search.Value.ToLower()))) ||
                    (parameters.Search.Value == null || (p.ProductCode != null && p.ProductCode.ToLower().Contains(parameters.Search.Value.ToLower())))
                );
            switch (parameters.Columns[parameters.Order[0].Column].Data)
            {
                case "productCode":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.ProductCode) : query.OrderByDescending(p => p.ProductCode);
                    break;
                case "price":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "date":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.Date) : query.OrderByDescending(p => p.Date);
                    break;
                case "enbled":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.Enabled) : query.OrderByDescending(p => p.Enabled);
                    break;
                case "userName":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.User.Name) : query.OrderByDescending(p => p.User.Name);
                    break;
                case "brandName":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.Brand.Name) : query.OrderByDescending(p => p.Brand.Name);
                    break;
                case "reviews":
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.Reviews) : query.OrderByDescending(p => p.Reviews);
                    break;
                case "name":
                default:
                    query = parameters.Order[0].Dir == OrderDir.ASC ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name);
                    break;
            }
            
            var products = query.Select(p => new ProductListModel
                {
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
                    Reviews = p.Reviews.ToString("n0"),
                    Categories = string.Join(", ", p.CategoryProducts.Select(p => p.Category.Name))
                });
                
            Result<ProductListModel> result = new Result<ProductListModel>
            {
                draw = parameters.Draw,
                data = await products.Skip(parameters.Start).Take(parameters.Length).ToListAsync(),
                recordsFiltered = query.Count(),
                recordsTotal = context.Products.Count()
            };
            return Json(result);
        }

        public async Task<IActionResult> Create()
        {
            var model = new Product { Enabled = true };
            await PopulateViewData();
            return View(model);
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
                if (model.PictureFiles != null)
                {
                    foreach (var pictureFile in model.PictureFiles)
                    {
                        try
                        {
                            using (var image = await Image.LoadAsync(pictureFile.OpenReadStream()))
                            {
                                image.Mutate(p => p.Resize(new ResizeOptions
                                {
                                    Size = new Size(800, 800)
                                }));
                                var producPicture = new ProductPicture { Picture = image.ToBase64String(PngFormat.Instance), Date = DateTime.Now, Enabled = true, UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id };
                                context.Entry(producPicture).State = EntityState.Added;
                                model.ProductPictures.Add(producPicture);
                            }
                        }
                        catch (UnknownImageFormatException)
                        {
                            TempData["error"] = "Yüklenen bir ya da daha fazla görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        }
                    }

                }
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.Price = decimal.Parse(model.PriceText, CultureInfo.CreateSpecificCulture("tr-TR"));

                if (model.SelectedCategoryIds != null)
                {
                    foreach (var selectedCategoryId in model.SelectedCategoryIds)
                    {
                        var categoryProduct = new CategoryProduct { CategoryId = selectedCategoryId };
                        context.Entry(categoryProduct).State = EntityState.Added;
                        model.CategoryProducts.Add(categoryProduct);
                    }
                }


                context.Entry(model).State = EntityState.Added;
                try
                {
                    await context.SaveChangesAsync();
                    TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    TempData["error"] = $"{model.Name} isimli başka bir {entityName.ToLower()} olduğu için ekleme işlemi tamamlanamıyor.";
                    await PopulateViewData();
                    return View(model);
                }
            }
            else
            {
                await PopulateViewData();
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var model = await context.Products.FindAsync(id);
            model.PriceText = model.Price.ToString("#.00");
            model.SelectedCategoryIds = model.CategoryProducts.Select(p => p.CategoryId).ToList();
            await PopulateViewData();
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
                
                var categoryProducts = context.CategoryProducts.Where(p => p.ProductId == model.Id).ToList();
                if (model.SelectedCategoryIds != null)
                {
                    foreach (var selectedCategoryId in model.SelectedCategoryIds.Where(p => !categoryProducts.Any(q => q.CategoryId == p)))
                    {
                        var categoryProduct = new CategoryProduct { CategoryId = selectedCategoryId };
                        context.Entry(categoryProduct).State = EntityState.Added;
                        model.CategoryProducts.Add(categoryProduct);
                    }
                }
                foreach (var categoryProduct in categoryProducts.Where(p => !model.SelectedCategoryIds.Any(q => q == p.CategoryId)))
                    context.Entry(categoryProduct).State = EntityState.Deleted;

                if (model.PictureFiles != null)
                {
                    foreach (var pictureFile in model.PictureFiles)
                    {
                        try
                        {
                            using (var image = await Image.LoadAsync(pictureFile.OpenReadStream()))
                            {
                                image.Mutate(p => p.Resize(new ResizeOptions
                                {
                                    Size = new Size(800, 800)
                                }));
                                var producPicture = new ProductPicture { Picture = image.ToBase64String(PngFormat.Instance), Date = DateTime.Now, Enabled = true, UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id };
                                context.Entry(producPicture).State = EntityState.Added;
                                model.ProductPictures.Add(producPicture);
                            }
                        }
                        catch (UnknownImageFormatException)
                        {
                            TempData["error"] = "Yüklenen bir ya da daha fazla görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        }
                    }

                }

                if (model.PicturesToDeleted != null)
                    foreach (var pictureId in model.PicturesToDeleted)
                        context.Entry(await context.ProductPictures.FindAsync(pictureId)).State = EntityState.Deleted;

                model.Price = decimal.Parse(model.PriceText, CultureInfo.CreateSpecificCulture("tr-TR"));
                context.Entry(model).State = EntityState.Modified;
                try
                {
                    await context.SaveChangesAsync();
                    TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
                    TempData["error"] = $"{model.Name} isimli başka bir {entityName.ToLower()} olduğu için ekleme işlemi tamamlanamıyor.";
                    await PopulateViewData();
                    return View(model);
                }
            }
            else
            {
                await PopulateViewData();
                return View(model);
            }
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

        private async Task PopulateViewData()
        {
            ViewData["Categories"] = new SelectList((await context.Categories.ToListAsync()).Select(p => new { p.Id, Name = string.Join(" / ", p.GetPathItems().Select(q => q.Name)) }), "Id", "Name");
            ViewData["Brands"] = new SelectList((await context.Brands.Select(p => new { p.Id, p.Name }).ToListAsync()), "Id", "Name");
        }
    }
}
