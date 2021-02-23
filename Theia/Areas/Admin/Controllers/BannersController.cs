using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheiaData;
using TheiaData.Data;

namespace Theia.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "ProductAdministrators")]
    public class BannersController : Controller
    {
        private readonly AppDbContext context;

        private readonly UserManager<User> userManager;

        private readonly string entityName = "Tanıtım Görseli";

        public BannersController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Banners.OrderBy(p => p.SortOrder).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            await PopulateViewData();
            return View(new Banner { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Banner model)
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
                                Size = new Size(960, 360)
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
                else
                {
                    TempData["error"] = $"Lütfen bir {entityName.ToLower()} yükleyiniz.";
                    await PopulateViewData();
                    return View(model);
                }
                var nextOrder = ((await context.Banners.OrderByDescending(_ => _.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.SortOrder = nextOrder;


                if (model.SelectedCategoryIds != null)
                {
                    foreach (var selectedCategoryId in model.SelectedCategoryIds)
                    {
                        var categoryProduct = new CategoryBanner { CategoryId = selectedCategoryId };
                        context.Entry(categoryProduct).State = EntityState.Added;
                        model.CategoryBanners.Add(categoryProduct);
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
                    await PopulateViewData();
                    return View(model);
                };
            }
            else
                return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var model = await context.Banners.FindAsync(id);
            await PopulateViewData();
            model.SelectedCategoryIds = model.CategoryBanners.Select(p => p.CategoryId).ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Banner model)
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
                                Size = new Size(960, 360)
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

                var categoryBanners = context.CategoryBanners.Where(p => p.BannerId == model.Id).ToList();
                if (model.SelectedCategoryIds != null)
                {
                    foreach (var selectedCategoryId in model.SelectedCategoryIds.Where(p => !categoryBanners.Any(q => q.CategoryId == p)))
                    {
                        var categoryBanner = new CategoryBanner { CategoryId = selectedCategoryId };
                        context.Entry(categoryBanner).State = EntityState.Added;
                        model.CategoryBanners.Add(categoryBanner);
                    }
                }
                foreach (var categoryBanner in categoryBanners.Where(p => !model.SelectedCategoryIds.Any(q => q == p.CategoryId)))
                    context.Entry(categoryBanner).State = EntityState.Deleted;


                context.Entry(model).State = EntityState.Modified;
                try
                {
                    await context.SaveChangesAsync();
                    TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır.";
                    return RedirectToAction("Index");
                }
                catch (DbUpdateException)
                {
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
            var model = await context.Banners.FindAsync(id);
            context.Entry(model).State = EntityState.Deleted;
            try
            {
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} silme işlemi başarıyla tamamlanmıştır.";
            }
            catch (DbUpdateException)
            {

            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> MoveUp(int id)
        {
            var subject = await context.Banners.FindAsync(id);
            var target = await context.Banners.Where(p => p.SortOrder < subject.SortOrder).OrderBy(p => p.SortOrder).LastOrDefaultAsync();
            if (target != null)
            {
                var m = target.SortOrder;
                target.SortOrder = subject.SortOrder;
                subject.SortOrder = m;
                context.Entry(subject).State = EntityState.Modified;
                context.Entry(target).State = EntityState.Modified;
                await context.SaveChangesAsync();
                TempData["success"] = "Sıralama işlemi başarıyla tamamlanmıştır";
            }
            return RedirectToAction("Index");
        }
        
        public async Task<IActionResult> MoveDn(int id)
        {
            var subject = await context.Banners.FindAsync(id);
            var target = await context.Banners.Where(p => p.SortOrder > subject.SortOrder).OrderBy(p => p.SortOrder).FirstOrDefaultAsync();
            if (target != null)
            {
                var m = target.SortOrder;
                target.SortOrder = subject.SortOrder;
                subject.SortOrder = m;
                context.Entry(subject).State = EntityState.Modified;
                context.Entry(target).State = EntityState.Modified;
                await context.SaveChangesAsync();
                TempData["success"] = "Sıralama işlemi başarıyla tamamlanmıştır";
            }
            return RedirectToAction("Index");
        }

        private async Task PopulateViewData()
        {
            ViewData["Categories"] = new SelectList((await context.Categories.ToListAsync()).Select(p => new { p.Id, Name = string.Join(" / ", p.GetPathItems().Select(q => q.Name)) }), "Id", "Name");
        }

    }
}
