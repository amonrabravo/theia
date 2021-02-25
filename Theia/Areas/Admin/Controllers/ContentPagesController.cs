using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheiaData;
using TheiaData.Data;

namespace Theia.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles = "Administrators")]
    public class ContentPagesController : Controller
    {
        private readonly AppDbContext context;

        private readonly UserManager<User> userManager;

        private readonly string entityName = "Marka";

        public ContentPagesController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.ContentPages.OrderBy(p => p.SortOrder).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new ContentPage { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ContentPage model)
        {
            if (ModelState.IsValid)
            {
                var nextOrder = ((await context.ContentPages.OrderByDescending(_ => _.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;
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
            return View(await context.ContentPages.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ContentPage model)
        {
            if (ModelState.IsValid)
            {
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
            var model = await context.ContentPages.FindAsync(id);
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
            var subject = await context.ContentPages.FindAsync(id);
            var target = await context.ContentPages.Where(p => p.SortOrder < subject.SortOrder).OrderBy(p => p.SortOrder).LastOrDefaultAsync();
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
            var subject = await context.ContentPages.FindAsync(id);
            var target = await context.ContentPages.Where(p => p.SortOrder > subject.SortOrder).OrderBy(p => p.SortOrder).FirstOrDefaultAsync();
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
