using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;

namespace Bulky.DataAccess.Repository {
    public class CategoryDataService : DataService<Category>, ICategoryDataService {

        public CategoryDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }

        public void Update(Category category) {
            dbContext.Categories.Update(category);
        }
    }
}

