using System;
namespace Bulky.Models.ViewModels {
	public class ShoppingCartViewModel {
		public IEnumerable<ShoppingCart> shoppingCartList { get; set; }
		public OrderHeader OrderHeader { get; set; }

        public double GetOrderTotal() {
			OrderHeader.OrderTotal = 0;

            double price = 0;
			foreach(var item in shoppingCartList) {
				price = 0;
				if(item.Count < 50) {
                    price = item.Product.Price;
				} else if(item.Count > 50 && item.Count <= 100) {
                    price = item.Product.Price50;
				} else {
                    price = item.Product.Price100;
                }
				item.Price = price;
				OrderHeader.OrderTotal += (item.Price * item.Count);
			}

			return OrderHeader.OrderTotal;
		}
	}
}

