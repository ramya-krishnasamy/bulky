using System;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IDataService;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Repository {

    public class DataService<T> : IDataService<T> where T : class {
        protected ApplicationDbContext dbContext;
        internal DbSet<T> dbSet;

        public DataService(ApplicationDbContext dbContext) {
            this.dbContext = dbContext;
            dbSet = this.dbContext.Set<T>();
            dbContext.products.Include(x => x.Category).Include(x => x.CategoryId);
        }

        public void Add(T entity) {
            dbSet.Add(entity);
        }

        public T Get(System.Linq.Expressions.Expression<Func<T, bool>> filter, string? includeProperties = null,bool isTracked = false) {
            IQueryable<T> query = isTracked ? dbSet : dbSet.AsNoTracking();
            query = query.Where(filter);

            if(!string.IsNullOrEmpty(includeProperties)) {
                foreach(var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProp);
                }
            }
            return query.FirstOrDefault();
        }

        public List<T> GetAll(System.Linq.Expressions.Expression<Func<T, bool>>? filter, string? includeProperties = null) {
            IQueryable<T> query = dbSet;
            if(filter != null)
                query = query.Where(filter);

            if(!string.IsNullOrEmpty(includeProperties)) {
                foreach(var includeProp in includeProperties
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
        }

        public void Remove(T entity) {
            dbSet.Remove(entity);
        }

        public void Remove(IEnumerable<T> values) {
            dbSet.RemoveRange(values);
        }
    }
}

