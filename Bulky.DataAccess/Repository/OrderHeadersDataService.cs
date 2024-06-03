using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;
using Bulky.Utility;
using Stripe.Climate;

namespace Bulky.DataAccess.Repository {
    public class OrderHeadersDataService : DataService<OrderHeader>, IOrderHeadersDataService {

        public OrderHeadersDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }

        public void Update(OrderHeader category) {
            dbContext.orderHeaders.Update(category);
        }

        public void UpdateStatus(int OrderId, string orderStatus, string? PaymentStatus = null) {
            var order = dbContext.orderHeaders.FirstOrDefault(x => x.Id == OrderId);
            if(order != null) {
                order.OrderStatus = orderStatus;
                if(!string.IsNullOrEmpty(PaymentStatus)) {
                    order.PaymentStatus = PaymentStatus;
                }
                dbContext.orderHeaders.Update(order);
                dbContext.SaveChanges();
            }
            
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId) {
            var order = dbContext.orderHeaders.FirstOrDefault(x => x.Id == id);
            if(!string.IsNullOrEmpty(sessionId)) {
                order.SessionId = sessionId;
            }
            if(!string.IsNullOrEmpty(paymentIntendId)) {
                order.PaymentIntendId = paymentIntendId;
                order.PaymentDate = DateTime.Now;

            }
        }
    }
}

