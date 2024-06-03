using System;
using Bulky.DataAccess.Repository.IDataService;

namespace Bulky.Models {
	public interface IOrderDetailsDataService : IDataService<OrderDetail> {
		void Update(OrderDetail category);
	}
}

