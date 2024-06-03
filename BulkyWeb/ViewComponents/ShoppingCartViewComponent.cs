using System;
using System.Security.Claims;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Utility;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.ViewComponents {
	public class ShoppingCartViewComponent : ViewComponent {

		private readonly IUnitOfWork unitOfWork;
		public ShoppingCartViewComponent(IUnitOfWork unitOfWork) {
			this.unitOfWork = unitOfWork;
		}

		public async Task<IViewComponentResult> InvokeAsync() {
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

			int count = 0;

			if(claim != null) {
				var cart = unitOfWork.shoppingCart.GetAll(x => x.UserId == claim.Value);
				count = cart.Count;
				HttpContext.Session.SetInt32(ApplicationConstants.SessionShoppingCart, count);
			} else {
				HttpContext.Session.Clear();
			}

			return View(count);
		}
	}
}

