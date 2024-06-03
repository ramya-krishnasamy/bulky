using System;
using Bulky.DataAccess.Repository.IDataService;

namespace Bulky.Models {
	public interface IProductImageDataService : IDataService<ProductImage> {
		void Update(ProductImage image);
	}
}

