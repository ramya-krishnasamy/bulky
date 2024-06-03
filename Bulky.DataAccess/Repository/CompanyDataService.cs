using System;
using Bulky.DataAccess.Data;
using Bulky.Models;

namespace Bulky.DataAccess.Repository.IDataService {
	public class CompanyDataService : DataService<Company>, ICompanyDataService {

		ApplicationDbContext dbContext;

		public CompanyDataService(ApplicationDbContext dbContext) : base(dbContext) {
			this.dbContext = dbContext;
		}

        public void Update(Company company) {
            dbContext.Companies.Update(company);
        }
    }
}