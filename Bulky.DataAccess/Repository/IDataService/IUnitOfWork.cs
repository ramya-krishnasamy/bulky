using System;
using Bulky.Models;

namespace Bulky.DataAccess.Repository.IDataService {

	public interface IUnitOfWork {
		ICategoryDataService category { get; }
        IProductDataService product { get; }
        ICompanyDataService companies { get; }
        IShoppingCartDataService shoppingCart { get; }
        IApplicationUserDataService applicationUserDataService { get; }
        IOrderDetailsDataService orderDetailsDataService { get; }
        IOrderHeadersDataService orderHeadersDataService { get; }
        IProductImageDataService productImageDataService { get; }
        IUserRoleDataService userRoleDataService { get; }
        void Save();
	}
}

