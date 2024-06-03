using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;

namespace Bulky.DataAccess.Repository {
    public class OrderDetailsDataService : DataService<OrderDetail>, IOrderDetailsDataService {

        public OrderDetailsDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }

        public void Update(OrderDetail category) {
            dbContext.orderDetails.Update(category);
        }
    }
}

