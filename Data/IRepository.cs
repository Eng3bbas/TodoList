using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoList.Helpers;

namespace TodoList.Data
{
   public interface IRepository<T> where T : class
    {
        public Task<PaginatedList<T>> All(int page = 1);
        public Task<T> Create(T entity);
        public Task<IList<T>> Create(IList<T> entities);
        public Task<T> Find(int id);
        public Task<T> Find(Guid id);
        public Task<T> Update(T entity);
        public Task Delete(T entity);
        public Task<IEnumerable<T>> Filter(Func<T, bool> predicate);
    }
}
