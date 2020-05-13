using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TodoList.Extenstions;
using TodoList.Helpers;

namespace TodoList.Data
{
    public class Repository<T>: IRepository<T> where T : class
    {
        private readonly Context ctx;
        private readonly DbSet<T> dbSet;
        public Repository(Context ctx)
        {
            this.ctx = ctx;
            dbSet = ctx.Set<T>();
        }
        public async Task<PaginatedList<T>> All(int page = 1)
        {
            return GetRelatedEntites().Paginate(page);
        }

        

        public async Task<T> Create(T entity)
        {
            T en = (await dbSet.AddAsync(entity)).Entity;
            await SaveChangesAsync();
            return en;
        }

        public async Task<IList<T>> Create(IList<T> entities)
        {
           await dbSet.AddRangeAsync(entities);
           return await dbSet.TakeLast(entities.Count).ToListAsync();
        }

        public async Task Delete(T entity)
        {
            ctx.Remove(entity);
            await SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> Filter(Func<T, bool> predicate)
        {
            return dbSet.Where(predicate);
        }

        public async Task<T> Find(int id)
        {
            var parameter = Expression.Parameter(typeof(T), "entitiy");
            var predicate = Expression.Lambda<Func<T, bool>>(
                   Expression.Equal(
                   Expression.PropertyOrField(parameter, "Id"),
                   Expression.Constant(id)
                   ),
                   parameter
                );
            try
            {
                return (await GetRelatedEntites().FirstAsync(predicate));

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<T> Find(Guid id)
        {
            var parameter = Expression.Parameter(typeof(T), "entitiy");
            var predicate = Expression.Lambda<Func<T, bool>>(
                   Expression.Equal(
                   Expression.PropertyOrField(parameter, "Id"),
                   Expression.Constant(id)
                   ),
                   parameter
                );

            try
            {
                return (await GetRelatedEntites().FirstAsync(predicate));

            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<T> Update(T entity)
        {
            T en =  dbSet.Update(entity).Entity;
            await SaveChangesAsync();
            return en;
        }


        private IQueryable<T> GetRelatedEntites()
        {
            
            var navigationProps = ctx.Model.FindEntityType(typeof(T)).GetNavigations();
            var query = dbSet.AsQueryable();
            navigationProps.ToList().ForEach(navProp =>
            {
                query = query.Include(navProp.Name);
            });
            return query;
        }
        private async Task SaveChangesAsync() => await ctx.SaveChangesAsync();
    }
}
