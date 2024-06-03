using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BulkyWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = ApplicationConstants.ROLE_ADMIN)]
    public class UserController : Controller {
        // GET: /<controller>/
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        public UserController(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager) {
            this.unitOfWork = unitOfWork;
            this._userManager = userManager;
            this.roleManager = roleManager;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult ManageRole(string id) {
            RoleManagmentVM UserRoleVM = new RoleManagmentVM {
                ApplicationUser = unitOfWork.applicationUserDataService.Get(x=>x.Id == id,includeProperties:"Company"),
                RoleList = roleManager.Roles.Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem {
                    Text = x.Name,
                    Value = x.Id
                }),
                CompanyList = unitOfWork.companies.GetAll().Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            var role = unitOfWork.userRoleDataService.Get(x => x.UserId == id);
            if(role != null) {
                UserRoleVM.ApplicationUser.Role = roleManager.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name;
            }

            return View(UserRoleVM);
        }

        [HttpPost]
        public IActionResult ManageRole(RoleManagmentVM roleManagmentVM) {

            string RoleID = unitOfWork.userRoleDataService.Get(u => u.UserId == roleManagmentVM.ApplicationUser.Id).RoleId;
            string oldRole = roleManager.Roles.FirstOrDefault(u => u.Id == RoleID).Name;

            if(!(roleManagmentVM.ApplicationUser.Role == oldRole)) {
                //a role was updated
                ApplicationUser applicationUser = unitOfWork.applicationUserDataService.Get(u => u.Id == roleManagmentVM.ApplicationUser.Id);
                if(roleManagmentVM.ApplicationUser.Role == ApplicationConstants.ROLE_COMPANY) {
                    applicationUser.CompanyId = roleManagmentVM.ApplicationUser.CompanyId;
                }
                if(oldRole == ApplicationConstants.ROLE_COMPANY) {
                    applicationUser.CompanyId = null;
                }
                unitOfWork.Save();

                _userManager.RemoveFromRoleAsync(applicationUser, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role).GetAwaiter().GetResult();

            }

            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() {
            var users = unitOfWork.applicationUserDataService.GetAll(includeProperties:"Company");
            users.ForEach(x => {
                if(x.Company == null) {
                    x.Company = new Bulky.Models.Company { Name = "" };
                }
                var role = unitOfWork.userRoleDataService.Get(y => y.UserId == x.Id);
                if(role != null) {
                    x.Role = roleManager.Roles.Where(x => x.Id == role.RoleId).FirstOrDefault().Name;
                }
            });
            return Json(new { data = users });
        }

        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id) {
            var user = unitOfWork.applicationUserDataService.Get(x => x.Id == id);
            if(user == null) {
                return Json(new { data = "No user available " });
            }

            if(user.LockoutEnd != null && user.LockoutEnd > DateTime.Now) {
                user.LockoutEnd = DateTime.Now;
            } else {
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            unitOfWork.Save();
            return Json(new { success = true, message = "User changes updated successfully" });
        }
       
        #endregion
    }
}

