using System;
using Bulky.DataAccess.Repository.IDataService;

namespace Bulky.Models {
	public interface IShoppingCartDataService : IDataService<ShoppingCart> {
		void Update(ShoppingCart shoppingCart);
	}
}

