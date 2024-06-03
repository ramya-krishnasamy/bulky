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
    public class EditModel : PageModel {

        ApplicationDbContext dbContext;
        public Category selectedCategory { get; set; }
        public EditModel(ApplicationDbContext db) {
            dbContext = db;
        }

        public void OnGet(int? id) {
            Category? category = dbContext.Categories.Find(id);
            if(category != null) {
                selectedCategory = category;
            }
        }

        public IActionResult OnPost() {
            if(ModelState.IsValid) {
                dbContext.Update(selectedCategory);
                dbContext.SaveChanges();
                TempData["success"] = "Category edited successfuly";
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
