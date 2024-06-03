using System;
using Bulky.DataAccess.Repository.IDataService;

namespace Bulky.Models {
	public interface IOrderHeadersDataService : IDataService<OrderHeader> {
		void Update(OrderHeader category);
        void UpdateStatus(int OrderId, string orderStatus, string? PaymentStatus = null);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId);
    }
}

