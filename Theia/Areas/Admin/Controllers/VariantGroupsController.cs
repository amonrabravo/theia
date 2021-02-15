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
    [Authorize(Roles = "ProductAdministrators")]
    public class VariantGroupsController : Controller
    {
        private readonly AppDbContext context;
        private readonly UserManager<User> userManager;

        private readonly string entityName = "Marka";

        public VariantGroupsController(AppDbContext context, UserManager<User> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await context.VariantGroups.OrderBy(p => p.SortOrder).ToListAsync());
        }

        public IActionResult Create()
        {
            return View(new VariantGroup { Enabled = true });
        }

        [HttpPost]
        public async Task<IActionResult> Create(VariantGroup model)
        {
            if (ModelState.IsValid)
            {
                var nextOrder = ((await context.VariantGroups.OrderByDescending(_ => _.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.SortOrder = nextOrder;

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
            return View(await context.VariantGroups.FindAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(VariantGroup model)
        {
            if (ModelState.IsValid)
            {
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
            var model = await context.VariantGroups.FindAsync(id);
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
            var subject = await context.VariantGroups.FindAsync(id);
            var target = await context.VariantGroups.Where(p => p.SortOrder < subject.SortOrder).OrderBy(p => p.SortOrder).LastOrDefaultAsync();
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
            var subject = await context.VariantGroups.FindAsync(id);
            var target = await context.VariantGroups.Where(p => p.SortOrder > subject.SortOrder).OrderBy(p => p.SortOrder).FirstOrDefaultAsync();
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
