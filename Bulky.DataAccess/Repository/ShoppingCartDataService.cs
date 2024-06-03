using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;

namespace Bulky.DataAccess.Repository {
    public class ShoppingCartDataService : DataService<ShoppingCart>, IShoppingCartDataService {

        public ShoppingCartDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }

        public void Update(ShoppingCart shoppingCart) {
            dbContext.shoppingCart.Update(shoppingCart);
        }
    }
}

