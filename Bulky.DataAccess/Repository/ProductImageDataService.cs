using System;
using System.Linq.Expressions;
using Bulky.DataAccess.Data;
using Bulky.Models;

namespace Bulky.DataAccess.Repository {
    public class ProductImageDataService : DataService<ProductImage>, IProductImageDataService {

        public ProductImageDataService(ApplicationDbContext dBcontext) : base(dBcontext) {
            this.dbContext = dBcontext;
        }

        public void Update(ProductImage product) {
            dbContext.productImages.Update(product);
        }
    }
}

