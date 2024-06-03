using System;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.DbInitializer {
    public class DBInitializer : IDBInitializer {
        public UserManager<IdentityUser> UserManager { get; set; }
        public RoleManager<IdentityRole> roleManager { get; set; }
        public ApplicationDbContext dbContext { get; set; }

        public DBInitializer(
            UserManager<IdentityUser> UserManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext) {
            this.UserManager = UserManager;
            this.roleManager = roleManager;
            this.dbContext = dbContext;
        }

        public void Initialize()
        {
            try {
                if(dbContext.Database.GetPendingMigrations().Count() > 0) {
                    dbContext.Database.Migrate();
                }
            }catch(Exception e) {

            }

            if(!roleManager.RoleExistsAsync(ApplicationConstants.ROLE_CUSTOMER).GetAwaiter().GetResult()) {
                roleManager.CreateAsync(new IdentityRole(ApplicationConstants.ROLE_CUSTOMER)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(ApplicationConstants.ROLE_ADMIN)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(ApplicationConstants.ROLE_COMPANY)).GetAwaiter().GetResult();
                roleManager.CreateAsync(new IdentityRole(ApplicationConstants.ROLE_EMPLOYEE)).GetAwaiter().GetResult();

                UserManager.CreateAsync(new ApplicationUser {
                    UserName = "ajaypandiyaraj@gmail.com",
                    Email = "ajaypandiyaraj@gmail.com",
                    EmailConfirmed = true,
                    Name = "ajey pandiyaraj",
                    PhoneNumber = "5195740975",
                    Address = "test 123 ave",
                    City = "Waterloo",
                    State = "Ontario",
                    PostalCode = "N2V 0E2",
                }, "Bustedlife@007").GetAwaiter().GetResult();

                var user = dbContext.applicationUsers.FirstOrDefault(x => x.Email == "ajaypandiyaraj@gmail.com");
                UserManager.AddToRoleAsync(user, ApplicationConstants.ROLE_ADMIN).GetAwaiter().GetResult();
            }

            return;
        }
    }
}

