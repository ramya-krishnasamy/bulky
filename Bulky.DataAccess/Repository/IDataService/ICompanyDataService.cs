using System;
using Bulky.Models;

namespace Bulky.DataAccess.Repository.IDataService {
	public interface ICompanyDataService : IDataService<Company> {
		void Update(Company obj);
	}
}

