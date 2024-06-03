using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IDataService;
using Bulky.Models;
using System.Linq;

namespace Bulky.DataAccess.Repository {
    public class ProductDataService : DataService<Product>, IProductDataService {

        public ProductDataService(ApplicationDbContext dbContext) : base(dbContext) {
            this.dbContext = dbContext;
        }

        public void Update(Product product) {
            var productFromDb = dbContext.products.FirstOrDefault(x => x.Id == product.Id);
            if(productFromDb != null) {
                productFromDb.Title = product.Title;
                productFromDb.Author = product.Author;
                productFromDb.ISBN = product.ISBN;
                productFromDb.ListPrice = product.ListPrice;
                productFromDb.Price = product.Price;
                productFromDb.Price50 = product.Price50;
                productFromDb.Price100 = product.Price100;
                productFromDb.CategoryId = product.CategoryId;
                productFromDb.Description = product.Description;
                productFromDb.productImages = product.productImages;
                //if(product.ImageUrl != null || string.IsNullOrEmpty(product.ImageUrl)) {
                //    productFromDb.ImageUrl = product.ImageUrl;
                //}
                dbContext.products.Update(productFromDb);
            }
        }
    }
}

