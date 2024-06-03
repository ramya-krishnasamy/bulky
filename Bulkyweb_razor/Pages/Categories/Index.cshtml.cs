using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulkyweb_razor.Data;
using Bulkyweb_razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulkyweb_razor.Pages.Categories
{
	public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext dbContext;
        public List<Category> categories { get; set; }

        public IndexModel(ApplicationDbContext db) {
            dbContext = db;
        }

        public void OnGet()
        {
            categories = dbContext.Categories.ToList();
        }
    }
}
