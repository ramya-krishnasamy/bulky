using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;

namespace Bulky.DataAccess.Repository {
    public class ApplicationUserDataService : DataService<ApplicationUser>, IApplicationUserDataService {

        public ApplicationUserDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }
    }
}

