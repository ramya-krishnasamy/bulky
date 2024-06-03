using System;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;

namespace Bulky.DataAccess.Repository {
    public class UnitOfWork : IUnitOfWork {
        public ICategoryDataService category { get; private set; }
        public IProductDataService product { get; private set; }
        public ICompanyDataService companies { get; private set; }
        public IShoppingCartDataService shoppingCart { get; private set; }
        public IApplicationUserDataService applicationUserDataService { get; private set; }
        public IOrderDetailsDataService orderDetailsDataService { get; private set; }
        public IOrderHeadersDataService orderHeadersDataService { get; private set; }
        public IProductImageDataService productImageDataService { get; private set; }
        public IUserRoleDataService userRoleDataService { get; private set; }

        ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
            applicationUserDataService = new ApplicationUserDataService(dbContext);
            category = new CategoryDataService(dbContext);
            product = new ProductDataService(dbContext);
            companies = new CompanyDataService(dbContext);
            shoppingCart = new ShoppingCartDataService(dbContext);
            orderDetailsDataService = new OrderDetailsDataService(dbContext);
            orderHeadersDataService = new OrderHeadersDataService(dbContext);
            productImageDataService = new ProductImageDataService(dbContext);
            userRoleDataService = new UserRoleDataService(dbContext);
        }

        public void Save() {
            dbContext.SaveChanges();
        }
    }
}

