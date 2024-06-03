using System;
using System.Linq.Expressions;

namespace Bulky.DataAccess.Repository.IDataService
{
	public interface IDataService<T> where T :class
	{
        List<T> GetAll(Expression<Func<T, bool>>? expression = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> expression, string? includeProperties = null, bool isTracked = false);
        void Add(T entity);
        void Remove(T entity);
        void Remove(IEnumerable<T> values);
    }
}

