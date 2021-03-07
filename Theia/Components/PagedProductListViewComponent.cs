using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using Theia.Models;
using TheiaData.Data;

namespace Theia.Components
{
    public class PagedProductListViewComponent : ViewComponent
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public PagedProductListViewComponent(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public IViewComponentResult Invoke(IQueryable<Product> products)
        {
            var absolutePageValue = httpContextAccessor.HttpContext.Request.Query["page"].ToString();
            int absolutePage = int.Parse(string.IsNullOrEmpty(absolutePageValue) ? "1" : absolutePageValue);
            var pageSizeValue = httpContextAccessor.HttpContext.Request.Query["pageSize"].ToString();
            int pageSize = int.Parse(string.IsNullOrEmpty(pageSizeValue) ? "12" : pageSizeValue);
            int totalCount = products.Count();
            var queries = httpContextAccessor.HttpContext.Request.Query;
            queries.Append(new KeyValuePair<string, StringValues>("page", "1"));
            var model = new ProductListViewModel
            {
                AbsolutePage = absolutePage,
                PageSize = pageSize,
                Products = products.Skip((absolutePage - 1) * pageSize).Take(pageSize).ToList(),
                TotalCount = totalCount,
                Pages = Enumerable.Range(1, (int)(Math.Ceiling(totalCount / (float)pageSize))).Select(p => new ProductListPage { Text = p.ToString(), Url = "" }).ToList()
            };
            return View(model);
        }
    }
}
