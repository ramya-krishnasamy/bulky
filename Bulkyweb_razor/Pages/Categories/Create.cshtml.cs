using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulkyweb_razor.Data;
using Bulkyweb_razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Bulkyweb_razor.Pages.Categories {
    [BindProperties]
    public class CreateModel : PageModel {
        ApplicationDbContext dbContext;
        public Category category { get; set; }

        public CreateModel(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
        }

        public void OnGet() {
        }

        public IActionResult OnPost() {
            if(ModelState.IsValid) {
                dbContext.Add(category);
                dbContext.SaveChanges();
                TempData["success"] = "Category created successfuly";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
