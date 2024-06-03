using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.DataAccess.Repository.IDataService;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bulky.Utility;
using Bulky.DataAccess.Repository;

namespace BulkyWeb.Area.Customer.Controllers {
    [Area("Customer")]
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            IEnumerable<Product> products = unitOfWork.product.GetAll(includeProperties:"Category,productImages");
            var index = HttpContext.Session.GetInt32(ApplicationConstants.SessionShoppingCart);
            return View(products);
        }

        public IActionResult Details(int? id) {
            if(id == 0 || id == null) {
                return NotFound();
            }

            Product product = unitOfWork.product.Get(x => x.Id == id, includeProperties:"Category,productImages");
            ShoppingCart shoppingCart = new ShoppingCart() {
                Product = product,
                Count = 1,
                ProductId = product.Id
            };

            if(product == null) {
                return NotFound();
            }

            return View(shoppingCart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details([FromForm] ShoppingCart shoppingCart) {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var userID = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCart.UserId = userID;

            var existingCart = unitOfWork.shoppingCart.Get(x => x.ProductId == shoppingCart.ProductId
            && x.UserId == shoppingCart.UserId, "Product,ApplicationUser");

            if(existingCart != null) {
                existingCart.Count += shoppingCart.Count;
                unitOfWork.shoppingCart.Update(existingCart);
            } else {
                unitOfWork.shoppingCart.Add(shoppingCart);
            }
            unitOfWork.Save();
            TempData["success"] = "Cart Updated successfully";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy() {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

