using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;
using Microsoft.AspNetCore.Identity;

namespace Bulky.DataAccess.Repository {
    public class UserRoleDataService : DataService<IdentityUserRole<string>>, IUserRoleDataService {

        public UserRoleDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }
    }
}

