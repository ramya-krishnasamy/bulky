using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulkyweb_razor.Data;
using Bulkyweb_razor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Bulkyweb_razor.Pages.Categories {
    [BindProperties]
    public class DeleteModel : PageModel {

        ApplicationDbContext DbContext;
        public Category selectedCategory { get; set; }

        public DeleteModel(ApplicationDbContext dbContext) {
            this.DbContext = dbContext;
        }

        public void OnGet(int? id) {
            Category? category = DbContext.Categories.Find(id);
            if(category != null) {
                selectedCategory = category;
            }
        }

        public IActionResult OnPost() {
            DbContext.Remove(selectedCategory);
            DbContext.SaveChanges();
            TempData["success"] = "Category deleted successfuly";
            return RedirectToPage("Index");
        }
    }
}
