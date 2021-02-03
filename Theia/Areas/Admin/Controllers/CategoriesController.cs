using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Theia.Data;

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
            return View(await context.Categories.Include(p => p.User).Where(p => p.ParentId == null).ToListAsync());
        }

        public IActionResult Create(int? parentId = null)
        {
            ViewBag.Path = Category.GetPath(context, parentId.Value);
            return View(new Category { Enabled = true, ParentId = parentId });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                var nextOrder = ((await context.Categories.Where(_ => _.ParentId == model.ParentId).OrderByDescending(_ => _.SortOrder).FirstOrDefaultAsync())?.SortOrder ?? 0) + 1;
                model.UserId = (await userManager.FindByNameAsync(User.Identity.Name)).Id;
                model.Date = DateTime.Now;
                model.SortOrder = nextOrder;

                context.Entry(model).State = EntityState.Added;
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} ekleme işlemi başarıyla tamamlanmıştır.";
                return model.ParentId == null ? RedirectToAction("Index", new { id = model.ParentId }) : RedirectToAction("Edit", new { id = model.ParentId });
            }
            else
                return View(model);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Path = Category.GetPath(context, id.Value);
            return View(await context.Categories.Include(_ => _.Children).Include(_ => _.User).SingleAsync(_ => _.Id == id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                TempData["success"] = $"{entityName} güncelleme işlemi başarıyla tamamlanmıştır.";
                return RedirectToAction("Index", new { id = model.ParentId });
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
    }
}
