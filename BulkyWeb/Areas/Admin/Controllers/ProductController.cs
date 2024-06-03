using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Bulky.DataAccess.Repository;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using System.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BulkyWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = ApplicationConstants.ROLE_ADMIN)]
    public class ProductController : Controller {
        // GET: /<controller>/
        private readonly IUnitOfWork unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) {
            this.unitOfWork = unitOfWork;
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index() {
            List<Product> objProductList = unitOfWork.product.GetAll(includeProperties: "Category");
            return View(objProductList);
        }

        public IActionResult Create() {
            ProductViewModel productVM = new ProductViewModel() {
                Product = new Product(),
                CategoryList = unitOfWork.category.GetAll().Select(x => new SelectListItem {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                })
            };

            return View(productVM);
        }

        public IActionResult Upsert(int? id) {
            ProductViewModel productVM = new ProductViewModel() {
                CategoryList = unitOfWork.category.GetAll().Select(x => new SelectListItem {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                })
            };

            if(id == 0 || id == null) {
                return View(productVM);
            } else {
                productVM.Product = unitOfWork.product.Get(x=>x.Id == id, includeProperties:"productImages");
                return View(productVM);
            }
        }

        public IActionResult DeleteImage(int id) {
            var Image = unitOfWork.productImageDataService.Get(x => x.Id == id);
            if(Image != null && !string.IsNullOrEmpty(Image.ImageUrl)) {
                string imagePath = $"{webHostEnvironment.WebRootPath}{Image.ImageUrl}";

                if(System.IO.File.Exists(imagePath)) {
                    System.IO.File.Delete(imagePath);
                }

                unitOfWork.productImageDataService.Remove(Image);
                unitOfWork.Save();

                TempData["success"] = "Product Image deleted successfuly";
            }
            return RedirectToAction(nameof(Upsert), new { id = Image.ProductId });
        }

        [HttpPost]
        public IActionResult Upsert(ProductViewModel productVM, List<IFormFile> formFiles) {
            ModelState.Remove("Product.Id");
            ModelState.Remove("formfile");
            if(ModelState.IsValid) {

                if(productVM.Product.Id != 0) {
                    unitOfWork.product.Update(productVM.Product);
                    TempData["success"] = "Product updated successfuly";
                } else {
                    unitOfWork.product.Add(productVM.Product);
                    TempData["success"] = "Product created successfuly";
                }

                unitOfWork.Save();

                if(formFiles != null) {
                    foreach(var file in formFiles) {
                        string productPath = $"Images/Products/product-{productVM.Product.Id}";
                        string imagePath = $"{webHostEnvironment.WebRootPath}/{productPath}";
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string filePath = $"{imagePath}/{fileName}";

                        if(!Directory.Exists(imagePath)) {
                            Directory.CreateDirectory(imagePath);
                        }
                        using(FileStream fileStream = new FileStream(filePath, FileMode.Create)) {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new ProductImage {
                            ImageUrl = $"/{productPath}/{fileName}",
                            ProductId = productVM.Product.Id
                        };
                        if(productVM.Product.productImages == null) {
                            productVM.Product.productImages = new();
                        }

                        productVM.Product.productImages.Add(productImage);
                    }
                    unitOfWork.product.Update(productVM.Product);
                    unitOfWork.Save();
                }
                
                return RedirectToAction(nameof(Index));
            }

            productVM.CategoryList = unitOfWork.category.GetAll().Select(x => new SelectListItem {
                Text = x.CategoryName,
                Value = x.Id.ToString()
            });
            return View(productVM);
        }

        public IActionResult Delete(int? id) {
            if(id == null || id == 0) {
                return NotFound();
            }

            var product = unitOfWork.product.Get(x => x.Id == id);

            if(product == null) {
                return NotFound();
            };
            return View(product);
        }

        //[ActionName("Delete")]
        //[HttpPost]
        //public IActionResult DeleteProduct(Product item) {
        //    var product = unitOfWork.product.Get(x => x.Id == item.Id);
        //    if(product != null) {


        //        if(!string.IsNullOrEmpty(product.ImageUrl)) {
        //            string imagePath = $"{webHostEnvironment.WebRootPath}+{item.ImageUrl}";

        //            if(System.IO.File.Exists(imagePath)) {
        //                System.IO.File.Delete(imagePath);
        //            }
        //        }

        //        TempData["success"] = "Product deleted successfuly";
        //        unitOfWork.product.Remove(product);
        //        unitOfWork.Save();
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll() {
            List<Product> objProductList = unitOfWork.product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }

        [HttpDelete]
        public IActionResult DeleteProduct(int? id) {
            Product product = unitOfWork.product.Get(x => x.Id == id);

            if(product == null) {
                return Json(new { success = false, message = "Product does not exists" });
            }

            if(product != null) {
                unitOfWork.product.Remove(product);
                unitOfWork.Save();

                string productPath = $"Images/Products/product-{product.Id}";
                string imagePath = $"{webHostEnvironment.WebRootPath}/{productPath}";

                if(Directory.Exists(imagePath)) {
                    var files = Directory.GetFiles(imagePath);
                    foreach(var file in files) {
                        System.IO.File.Delete(file);
                    }
                    Directory.Delete(imagePath);
                }
            } 
            return Json(new { success = true , message="Product successfuly deleted"});
        }
        #endregion
    }
}
