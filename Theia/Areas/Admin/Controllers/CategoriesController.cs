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
    public class CategoriesController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        private readonly string entityName = "Kategori";

        public CategoriesController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Categories.Where(p => p.ParentId == null).OrderBy(p => p.SortOrder).ToListAsync());
        }

        public async Task<IActionResult> Create(int? parentId = null)
        {
            await PopulateViewData();
            ViewBag.Path = Category.GetPath(context, parentId);
            return View(new Category { Enabled = true, ParentId = parentId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
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
                                Size = new Size(320, 240)
                            }));
                            model.Picture = image.ToBase64String(PngFormat.Instance);
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        TempData["error"] = "Yüklenen görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        await PopulateViewData();
                        return View(model);
                    }
                }

                if (model.SelectedVariantGroupIds != null)
                {
                    foreach (var selectedCategoryId in model.SelectedVariantGroupIds)
                    {
                        var variantGroupProduct = new CategoryVariantGroup { CategoryId = selectedCategoryId };
                        context.Entry(variantGroupProduct).State = EntityState.Added;
                        model.CategoryVariantGroups.Add(variantGroupProduct);
                    }
                }
                var nextOrder = ((await context.Categories.Where(_ => _.ParentId == model.ParentId).OrderByDescending(_ => _.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.SortOrder = nextOrder;

                context.Entry(model).State = EntityState.Added;
                try
                {
                    await context.SaveChangesAsync();
                    TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır.";
                    return model.ParentId == null ? RedirectToAction("Index", new { id = model.ParentId }) : RedirectToAction("Edit", new { id = model.ParentId });
                }
                catch (DbUpdateException)
                {
                    TempData["error"] = $"{model.Name} isimli başka bir kategori olduğu için ekleme işlemi tamamlanamıyor.";
                    await PopulateViewData();
                    return View(model);
                }
            }
            else
                return View(model);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Path = Category.GetPath(context, id.Value);
            return View(await context.Categories.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
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
                                Size = new Size(320, 240)
                            }));
                            model.Picture = image.ToBase64String(PngFormat.Instance);
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        TempData["error"] = "Yüklenen görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        await PopulateViewData();
                        return View(model);
                    }
                }

                var variantGroupsProducts = context.CategoryProducts.Where(p => p.ProductId == model.Id).ToList();
                if (model.SelectedVariantGroupIds != null)
                {
                    foreach (var selectedVariantGroupId in model.SelectedVariantGroupIds.Where(p => !variantGroupsProducts.Any(q => q.CategoryId == p)))
                    {
                        var categoryProduct = new CategoryProduct { CategoryId = selectedVariantGroupId };
                        context.Entry(categoryProduct).State = EntityState.Added;
                        model.CategoryProducts.Add(categoryProduct);
                    }
                }
                foreach (var categoryVariantGroup in variantGroupsProducts.Where(p => !model.SelectedVariantGroupIds.Any(q => q == p.CategoryId)))
                    context.Entry(categoryVariantGroup).State = EntityState.Deleted;

                context.Entry(model).State = EntityState.Modified;
                
                try
                {
                    await context.SaveChangesAsync();
                    TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır.";
                    return model.ParentId == null ? RedirectToAction("Index", new { id = model.ParentId }) : RedirectToAction("Edit", new { id = model.ParentId });
                }
                catch (DbUpdateException)
                {
                    TempData["error"] = $"{model.Name} isimli başka bir kategori olduğu için ekleme işlemi tamamlanamıyor.";
                    await PopulateViewData();
                    return View(model);
                }
            }
            else
                return View(model);
        }
        
        public async Task<IActionResult> Remove(int id)
        {
            var model = await context.Categories.FindAsync(id);
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
            return model.ParentId == null ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = model.ParentId });
        }

        
        public async Task<IActionResult> MoveUp(int id)
        {
            var subject = await context.Categories.FindAsync(id);
            var target = await context.Categories.Where(p => p.ParentId == subject.ParentId && p.SortOrder < subject.SortOrder).OrderBy(p => p.SortOrder).LastOrDefaultAsync();
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
            return subject.ParentId == null ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = subject.ParentId });
        }
        
        public async Task<IActionResult> MoveDn(int id)
        {
            var subject = await context.Categories.FindAsync(id);
            var target = await context.Categories.Where(p => p.ParentId == subject.ParentId && p.SortOrder > subject.SortOrder).OrderBy(p => p.SortOrder).FirstOrDefaultAsync();
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
            return subject.ParentId == null ? RedirectToAction("Index") : RedirectToAction("Edit", new { id = subject.ParentId });
        }
        
        private async Task PopulateViewData()
        {
            ViewData["VariantGroups"] = new SelectList((await context.VariantGroups.Select(p => new { p.Id, p.Name }).ToListAsync()), "Id", "Name");
        }
    }
}
