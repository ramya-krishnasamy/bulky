using System;
using Bulky.DataAccess.Repository.IDataService;
using Microsoft.AspNetCore.Identity;

namespace Bulky.Models {
	public interface IUserRoleDataService : IDataService<IdentityUserRole<string>> {
	}
}

