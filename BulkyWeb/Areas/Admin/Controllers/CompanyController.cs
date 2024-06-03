using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BulkyWeb.Areas.Admin.Controllers {
    [Area("Admin")]
    [Authorize(Roles = ApplicationConstants.ROLE_ADMIN)]
    public class CompanyController : Controller {
        // GET: /<controller>/
        private readonly IUnitOfWork unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
        }

        public IActionResult Index() {
            var companies = unitOfWork.companies.GetAll().ToList();
            return View(companies);
        }

        public IActionResult Upsert(int? id) {
            if(id == 0 || id == null) {
                return View(new Company());
            }

            var company = unitOfWork.companies.Get(x=>x.Id == id);
            if(company == null) {
                NotFound();
            }
            return View(company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company) {
            if(ModelState.IsValid) {
                if(company.Id == 0) {
                    unitOfWork.companies.Add(company);
                } else {
                    unitOfWork.companies.Update(company);
                }
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? id) {
            if(id == null || id == 0) {
                return NotFound();
            }

            var company = unitOfWork.companies.Get(x => x.Id == id);

            if(company == null) {
                return NotFound();
            };
            return View(company);
        }

        [HttpPost]
        public IActionResult Delete(Company companyObj) {
            var company = unitOfWork.companies.Get(x => x.Id == companyObj.Id);

            if(company == null) {
                return NotFound();
            };

            unitOfWork.companies.Remove(company);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}

