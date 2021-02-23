﻿using Microsoft.AspNetCore.Authorization;
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
    public class VariantsController : Controller
    {
        private readonly AppDbContext context;

        private readonly UserManager<User> userManager;

        private readonly string entityName = "Marka";

        public VariantsController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.Variants.OrderBy(p => p.VariantGroupId).ThenBy(p => p.SortOrder).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewData["VariantGroups"] = new SelectList(await context.VariantGroups.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            return View(new Variant { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Variant model)
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
                                Size = new Size(32, 32)
                            }));
                            model.Picture = image.ToBase64String(PngFormat.Instance);
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        ViewData["VariantGroups"] = new SelectList(await context.VariantGroups.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
                        TempData["error"] = "Yüklenen görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        return View(model);
                    }
                }
                var nextOrder = ((await context.Variants.OrderByDescending(_ => _.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.SortOrder = nextOrder;

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
                    return View(model);
                }
            }
            else
                return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["VariantGroups"] = new SelectList(await context.VariantGroups.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
            return View(await context.Variants.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Variant model)
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
                                Size = new Size(32, 32)
                            }));
                            model.Picture = image.ToBase64String(PngFormat.Instance);
                        }
                    }
                    catch (UnknownImageFormatException)
                    {
                        ViewData["VariantGroups"] = new SelectList(await context.VariantGroups.OrderBy(p => p.Name).ToListAsync(), "Id", "Name");
                        TempData["error"] = "Yüklenen görsel dosyası, işlenebilir bir görsel biçimi değil. Lütfen, PNG, JPEG, BMP, TIF biçimli görsel dosyaları yükleyiniz...";
                        return View(model);
                    }
                }
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
                    return View(model);
                }
            }
            else
                return View(model);
        }

        public async Task<IActionResult> Remove(int id)
        {
            var model = await context.Variants.FindAsync(id);
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

        public async Task<IActionResult> MoveUp(int id)
        {
            var subject = await context.Variants.FindAsync(id);
            var target = await context.Variants.Where(p => p.VariantGroupId == subject.VariantGroupId && p.SortOrder < subject.SortOrder).OrderBy(p => p.SortOrder).LastOrDefaultAsync();
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
            var subject = await context.Variants.FindAsync(id);
            var target = await context.Variants.Where(p => p.VariantGroupId == subject.VariantGroupId && p.SortOrder > subject.SortOrder).OrderBy(p => p.SortOrder).FirstOrDefaultAsync();
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
    }
}
