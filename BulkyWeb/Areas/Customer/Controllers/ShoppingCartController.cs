using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BulkyWeb.Areas.Customer.Controllers {
    [Area("Customer")]
    [Authorize]
    public class ShoppingCartController : Controller {
        // GET: /<controller>/
        [BindProperty]
        public ShoppingCartViewModel ShoppingCartViewModel { get; set; }

        private readonly IUnitOfWork unitOfWork;
        public ShoppingCartController(IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartViewModel sc = new ShoppingCartViewModel {
                shoppingCartList = unitOfWork.shoppingCart.GetAll(x => x.UserId == userId, "Product,ApplicationUser"),
                OrderHeader = new()
            };
            foreach(var item in sc.shoppingCartList) {
                item.Product.productImages = unitOfWork.productImageDataService.GetAll(x => x.ProductId == item.ProductId);
            }
            sc.GetOrderTotal();
            return View(sc);
        }

        public IActionResult Plus(int? cartId) {
            var shoppingCart = unitOfWork.shoppingCart.Get(x => x.Id == cartId);
            shoppingCart.Count += 1;
            unitOfWork.shoppingCart.Update(shoppingCart);
            unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int? cartId) {
            var shoppingCart = unitOfWork.shoppingCart.Get(x => x.Id == cartId);
            if(shoppingCart.Count <= 1) {
                unitOfWork.shoppingCart.Remove(shoppingCart);
            } else {
                shoppingCart.Count -= 1;
                unitOfWork.shoppingCart.Update(shoppingCart);
            }
            unitOfWork.Save();
            //var cart = unitOfWork.shoppingCart.GetAll(x => x.UserId == shoppingCart.UserId);
            //if(cart != null && cart.Count >= 0) {
            //    HttpContext.Session.SetInt32(ApplicationConstants.SessionShoppingCart, cart.Count);
            //}
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int? cartId) {
            var shoppingCart = unitOfWork.shoppingCart.Get(x => x.Id == cartId);
            unitOfWork.shoppingCart.Remove(shoppingCart);
            unitOfWork.Save();
            //var cart = unitOfWork.shoppingCart.GetAll(x => x.UserId == shoppingCart.UserId);
            //if(cart != null && cart.Count >= 0) {
            //    HttpContext.Session.SetInt32(ApplicationConstants.SessionShoppingCart, cart.Count);
            //} else {
            //    HttpContext.Session.SetInt32(ApplicationConstants.SessionShoppingCart, 0);
            //}
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary(int? cartId) {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userInfo = unitOfWork.applicationUserDataService.Get(x => x.Id == userId);
            ShoppingCartViewModel sc = new ShoppingCartViewModel {
                shoppingCartList = unitOfWork.shoppingCart.GetAll(x => x.UserId == userId, "Product,ApplicationUser"),
                OrderHeader = new() {
                    PhoneNumber = userInfo.PhoneNumber,
                    Address = userInfo.Address,
                    City = userInfo.City,
                    Province = userInfo.State,
                    Name = userInfo.Name,
                    PostalCode = userInfo.PostalCode
                }
            };
            sc.GetOrderTotal();
            return View(sc);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost() {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userInfo = unitOfWork.applicationUserDataService.Get(x => x.Id == userId);
            ShoppingCartViewModel.shoppingCartList = unitOfWork.shoppingCart.GetAll(x => x.UserId == userId, "Product");
            ShoppingCartViewModel.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartViewModel.OrderHeader.UserId = userId;

            ShoppingCartViewModel.GetOrderTotal();

            if(userInfo.CompanyId.GetValueOrDefault() == 0) {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = PaymentStatus.PENDING;
                ShoppingCartViewModel.OrderHeader.OrderStatus = OrderStatus.PENDING;
            } else {
                ShoppingCartViewModel.OrderHeader.PaymentStatus = PaymentStatus.DELAYED_PAYMENT;
                ShoppingCartViewModel.OrderHeader.OrderStatus = OrderStatus.PENDING;
            }

            unitOfWork.orderHeadersDataService.Add(ShoppingCartViewModel.OrderHeader);
            unitOfWork.Save();

            foreach(var item in ShoppingCartViewModel.shoppingCartList) {
                OrderDetail orderDetail = new OrderDetail() {
                    ProductId = item.ProductId,
                    OrderHeaderId = ShoppingCartViewModel.OrderHeader.Id,
                    Price = item.Price,
                    Count = item.Count
                };
                unitOfWork.orderDetailsDataService.Add(orderDetail);
                unitOfWork.Save();
            }

            if(userInfo.CompanyId.GetValueOrDefault() == 0) {
                var domain = $"{Request.Scheme}://{Request.Host.Value}/";
                var options = new Stripe.Checkout.SessionCreateOptions {
                    SuccessUrl = $"{domain}/Customer/ShoppingCart/OrderConfirmation?id={ShoppingCartViewModel.OrderHeader.Id}",
                    CancelUrl = $"{domain}/Customer/ShoppingCart/Index",
                    Mode = "payment",
                };

                var lineItems = new List<Stripe.Checkout.SessionLineItemOptions>();
                foreach(var items in ShoppingCartViewModel.shoppingCartList) {
                    lineItems.Add(new Stripe.Checkout.SessionLineItemOptions {
                        PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions {
                            UnitAmount = (long)(items.Price * 100),
                            Currency = "inr",
                            ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions {
                                Name = items.Product.Title,
                            }
                        },
                        Quantity = items.Count
                    });
                }

                options.LineItems = lineItems;

                var service = new SessionService();
                var session = service.Create(options);

                unitOfWork.orderHeadersDataService.UpdateStripePaymentId(ShoppingCartViewModel.OrderHeader.Id, session.Id, session.PaymentIntentId);
                unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartViewModel.OrderHeader.Id });
        }

        public IActionResult OrderConfirmation(int id) {
            var orderHeader = unitOfWork.orderHeadersDataService.Get(x => x.Id == id,includeProperties:"ApplicationUser");
            if(orderHeader.PaymentStatus != PaymentStatus.DELAYED_PAYMENT) {
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower() == "paid") {
                    unitOfWork.orderHeadersDataService.UpdateStripePaymentId(id, session.Id, session.PaymentIntentId);
                    unitOfWork.orderHeadersDataService.UpdateStatus(id, OrderStatus.APPROVED, PaymentStatus.APPROVED);
                    unitOfWork.Save();
                }
            } 

            List<ShoppingCart> shoppingCarts = unitOfWork.shoppingCart.GetAll(x => x.UserId == orderHeader.UserId).ToList();
            unitOfWork.shoppingCart.Remove(shoppingCarts);
            unitOfWork.Save();
            return View(id);
        }
    }
}

