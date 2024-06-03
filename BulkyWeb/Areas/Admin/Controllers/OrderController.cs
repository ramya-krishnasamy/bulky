using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BulkyWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller {
        // GET: /<controller>/
        private readonly IUnitOfWork unitOfWork;
        [BindProperty]
        public OrderViewModel OrderVM { get; set; }
        public OrderController(IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Details(int id) {
            OrderViewModel orderVM = new OrderViewModel {
                OrderHeader = unitOfWork.orderHeadersDataService.Get(x => x.Id == id, includeProperties:"ApplicationUser"),
                OrderDetail = unitOfWork.orderDetailsDataService.GetAll(x => x.OrderHeaderId == id,includeProperties:"Product")
            };
            return View(orderVM);
        }

        [HttpPost]
        [Authorize(Roles = $"{ApplicationConstants.ROLE_ADMIN},{ApplicationConstants.ROLE_EMPLOYEE}")]
        public IActionResult UpdateOrderDetails() {
            var orderHeader = unitOfWork.orderHeadersDataService.Get(x => x.Id == OrderVM.OrderHeader.Id);
            orderHeader.Address = orderHeader.Address;
            orderHeader.City = orderHeader.City;
            orderHeader.PostalCode = orderHeader.PostalCode;
            orderHeader.Province = orderHeader.Province;
            orderHeader.Name = orderHeader.Name;
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier)) {
                orderHeader.Carrier = OrderVM.OrderHeader.Carrier;
            }
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber)) {
                orderHeader.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }
            unitOfWork.orderHeadersDataService.Update(orderHeader);
            TempData["success"] = "Order Details updated";
            return RedirectToAction(nameof(Details), new { id = orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{ApplicationConstants.ROLE_ADMIN},{ApplicationConstants.ROLE_EMPLOYEE}")]
        public IActionResult StartProcess() {
            unitOfWork.orderHeadersDataService.UpdateStatus(OrderVM.OrderHeader.Id, OrderStatus.IN_PROCESS);
            unitOfWork.Save();
            TempData["success"] = "Order status updated succesfuly";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{ApplicationConstants.ROLE_ADMIN},{ApplicationConstants.ROLE_EMPLOYEE}")]
        public IActionResult ShipOrder() {
            if(!string.IsNullOrEmpty(OrderVM.OrderHeader.Carrier) && !string.IsNullOrEmpty(OrderVM.OrderHeader.TrackingNumber)) {
                var order = unitOfWork.orderHeadersDataService.Get(x => x.Id == OrderVM.OrderHeader.Id);
                order.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
                order.ShippingDate = DateTime.Now;
                order.OrderStatus = OrderStatus.SHIPPED;
                order.Carrier = OrderVM.OrderHeader.Carrier;
                if(order.PaymentStatus == PaymentStatus.DELAYED_PAYMENT) {
                    order.PaymentDueDate = DateTime.Now.AddDays(30);
                }
                unitOfWork.orderHeadersDataService.Update(order);
                unitOfWork.Save();
                TempData["success"] = "Order shipped succesfuly";
            } else {
                TempData["error"] = "Order cannot be shipped";
            }
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = $"{ApplicationConstants.ROLE_ADMIN},{ApplicationConstants.ROLE_EMPLOYEE}")]
        public IActionResult CancelOrder() {
            var order = unitOfWork.orderHeadersDataService.Get(x=>x.Id == OrderVM.OrderHeader.Id);
            if(order.PaymentStatus == PaymentStatus.APPROVED && !string.IsNullOrEmpty(order.PaymentIntendId)) {
                var refundOptions = new RefundCreateOptions {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = order.PaymentIntendId
                };

                var service = new RefundService();
                var refund = service.Create(refundOptions);

                unitOfWork.orderHeadersDataService.UpdateStatus(OrderVM.OrderHeader.Id, OrderStatus.CANCELLED, OrderStatus.REFUNDED);
            } else {
                unitOfWork.orderHeadersDataService.UpdateStatus(OrderVM.OrderHeader.Id, OrderStatus.CANCELLED, OrderStatus.CANCELLED);
            }
            unitOfWork.Save();
            TempData["success"] = "Order status updated succesfuly";
            return RedirectToAction(nameof(Details), new { id = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [ActionName("PayNow")]
        public IActionResult DetailsPayNow() {
            OrderVM.OrderHeader = unitOfWork.orderHeadersDataService.Get(x => x.Id == OrderVM.OrderHeader.Id,includeProperties:"ApplicationUser");
            OrderVM.OrderDetail = unitOfWork.orderDetailsDataService.GetAll(x => x.OrderHeaderId == OrderVM.OrderHeader.Id,"Product");
            var domain = $"{Request.Scheme}://{Request.Host.Value}/";
            var options = new Stripe.Checkout.SessionCreateOptions {
                SuccessUrl = $"{domain}/admin/order/OrderConfirmation?orderHeaderId={OrderVM.OrderHeader.Id}",
                CancelUrl = $"{domain}/admin/order/details?id={OrderVM.OrderHeader.Id}",
                Mode = "payment",
            };

            var lineItems = new List<Stripe.Checkout.SessionLineItemOptions>();
            foreach(var items in OrderVM.OrderDetail) {
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

            unitOfWork.orderHeadersDataService.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            unitOfWork.Save();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        public IActionResult OrderConfirmation(int orderHeaderId) {
            var orderHeader = unitOfWork.orderHeadersDataService.Get(x => x.Id == orderHeaderId);
            if(orderHeader.PaymentStatus == PaymentStatus.DELAYED_PAYMENT) {
                var service = new SessionService();
                var session = service.Get(orderHeader.SessionId);
                if(session.PaymentStatus.ToLower() == "paid") {
                    unitOfWork.orderHeadersDataService.UpdateStripePaymentId(orderHeaderId, session.Id, session.PaymentIntentId);
                    unitOfWork.orderHeadersDataService.UpdateStatus(orderHeaderId, orderHeader.OrderStatus, PaymentStatus.APPROVED);
                    unitOfWork.Save();
                }
            }

            unitOfWork.Save();
            return View(orderHeaderId);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status) {
            List<OrderHeader> orders;

            if(User.IsInRole(ApplicationConstants.ROLE_ADMIN) || User.IsInRole(ApplicationConstants.ROLE_EMPLOYEE)) {
                orders = unitOfWork.orderHeadersDataService.GetAll(includeProperties: "ApplicationUser").ToList();
            } else {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                orders = unitOfWork.orderHeadersDataService.GetAll(x => x.UserId == userId, includeProperties: "ApplicationUser");
            }

            if(!string.IsNullOrEmpty(status)) {
                switch(status) {
                    case "in_process":
                        orders = orders.Where(x=>x.OrderStatus == OrderStatus.IN_PROCESS).ToList();
                        break;
                    case "pending":
                        orders = orders.Where(x => x.PaymentStatus == PaymentStatus.PENDING).ToList();
                        break;
                    case "completed":
                        orders = orders.Where(x => x.OrderStatus == OrderStatus.SHIPPED).ToList();
                        break;
                    case "approved":
                        orders = orders.Where(x => x.OrderStatus == OrderStatus.APPROVED).ToList();
                        break;
                }
            }
            return Json(new { data = orders });
        }
        #endregion
    }
}
