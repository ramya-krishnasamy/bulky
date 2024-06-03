using System;
using Bulky.Models;

namespace Bulky.DataAccess.Repository.IDataService {
	public interface IProductDataService : IDataService<Product> {
		void Update(Product product);
	}
}

