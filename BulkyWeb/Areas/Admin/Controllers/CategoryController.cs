using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Area.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = ApplicationConstants.ROLE_ADMIN)]
    public class CategoryController : Controller {
        private readonly IUnitOfWork unitOfWork;
        public CategoryController(IUnitOfWork dbContext) {
            this.unitOfWork = dbContext;
        }

        public IActionResult Index() {
            List<Category> categoryList = unitOfWork.category.GetAll();
            return View(categoryList);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category) {
            if(ModelState.IsValid) {
                unitOfWork.category.Add(category);
                unitOfWork.Save();

                TempData["success"] = "Category created successfuly";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? id) {
            if(id == null || id == 0) {
                return NotFound();
            }

            Category? category = unitOfWork.category.Get(x=>x.Id == id);
            if(category == null) {
                NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category) {
            if(ModelState.IsValid) {
                unitOfWork.category.Update(category);
                unitOfWork.Save();
                TempData["success"] = "Category updated successfuly";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id) {
            if(id == null || id == 0) {
                return NotFound();
            }

            Category? category = unitOfWork.category.Get(x=>x.Id == id);
            if(category == null) {
                NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Delete(Category category) {
            unitOfWork.category.Remove(category);
            unitOfWork.Save();
            TempData["success"] = "Category deleted successfuly";

            return RedirectToAction("Index");
        }
    }
}