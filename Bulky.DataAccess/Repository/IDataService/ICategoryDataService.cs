using System;
using Bulky.DataAccess.Repository.IDataService;

namespace Bulky.Models {
	public interface ICategoryDataService : IDataService<Category> {
		void Update(Category category);
	}
}

