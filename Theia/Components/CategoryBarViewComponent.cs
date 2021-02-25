using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TheiaData;

public class CategoryBarViewComponent : ViewComponent
{
    private readonly AppDbContext context;

    public CategoryBarViewComponent(AppDbContext context)
    {
        this.context = context;
    }

    public IViewComponentResult Invoke()
    {
        var model = context.Categories.Where(p => p.Enabled && p.ParentId == null).OrderBy(p=>p.SortOrder).ToList();
        return View(model);
    }
}