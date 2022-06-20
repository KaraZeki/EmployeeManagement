using Microsoft.EntityFrameworkCore;
using Monovi.DataAccess.Context;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Monovi.DataAccess.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MonoviDbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(MonoviDbContext dbContext)
        {
            _dbContext = dbContext;
            this._dbSet = dbContext.Set<T>();
        }



        public List<T> FindAll<T>(List<T> list, List<Predicate<T>> predicates)
        {
            List<T> L = new List<T>();

            foreach (T item in list)
            {
                bool pass = true;
                foreach (Predicate<T> p in predicates)
                {
                    if (!(p(item)))
                    {
                        pass = false;
                        break;
                    }
                }

                if (pass)
                    L.Add(item);
            }

            return L;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> WhereAsync<T>(IEnumerable<T> source, Func<T, Task<bool>> predicate)
        {
            var results = new ConcurrentQueue<T>();
            var tasks = source.Select(
                async x =>
                {
                    if (await predicate(x))
                        results.Enqueue(x);
                });
            await Task.WhenAll(tasks);
            return results;
        }




        private IQueryable<T> Limit<T>(IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            int m_Skip = (pageNumber - 1) * pageSize;
            int m_PageSize = pageSize;

            if (m_Skip > m_PageSize)
                return queryable.Take(m_PageSize);
            else
                return queryable.Skip(m_Skip).Take(m_PageSize);
        }





        public IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] values)
        {
            var resultExp = CreateMethodCallExpression(source, "OrderBy", ordering);
            return source.Provider.CreateQuery<T>(resultExp);
        }

        public IQueryable<T> OrderByDescending<T>(IQueryable<T> source, string ordering, params object[] values)
        {
            var resultExp = CreateMethodCallExpression(source, "OrderByDescending", ordering);
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="ordering"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IQueryable<T> ThenBy<T>(IQueryable<T> source, string ordering, params object[] values)
        {
            var resultExp = CreateMethodCallExpression(source, "ThenBy", ordering);
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="ordering"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public IQueryable<T> ThenByDescending<T>(IQueryable<T> source, string ordering, params object[] values)
        {
            var resultExp = CreateMethodCallExpression(source, "ThenByDescending", ordering);
            return source.Provider.CreateQuery<T>(resultExp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filters"></param>
        /// <returns></returns>


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="methodName"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        private MethodCallExpression CreateMethodCallExpression<T>(IQueryable<T> source, string methodName, string ordering)
        {
            var strings = ordering.Split('.');

            var types = new List<Type>();
            var properties = new List<PropertyInfo>();
            var propertyAccesses = new List<MemberExpression>();

            types.Add(typeof(T));

            for (int i = 0; i < strings.Length; i++)
            {
                if (i != 0)
                    types.Add(properties[i - 1].PropertyType);

                properties.Add(types[i].GetProperty(strings[i]));
            }

            var parameter = Expression.Parameter(types[0], "p");

            for (int i = 0; i < properties.Count; i++)
            {
                propertyAccesses.Add(i == 0
                    ? Expression.MakeMemberAccess(parameter, properties[i])
                    : Expression.MakeMemberAccess(propertyAccesses[i - 1], properties[i]));
            }

            var orderByExp = Expression.Lambda(propertyAccesses.Last(), parameter);

            return Expression.Call(typeof(Queryable), methodName,
                new Type[] { types.First(), properties.Last().PropertyType }, source.Expression, Expression.Quote(orderByExp));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetById(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<T>> ListAllAsync()
        {
            //_dbContext.Database.IsOracle()
            return await _dbContext.Set<T>().ToListAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<T> ListAll()
        {
            //_dbContext.Database.IsOracle()
            return _dbContext.Set<T>().ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> GetAll()
        {
            try
            {
                return _dbContext.Set<T>();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            //return _dbContext.Set<T>();
        }

        /// <summary>
        /// Add Sync
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            var result = _dbContext.SaveChanges();

            if (result > 0)
                return entity;
            else
                return null;
        }

        /// <summary>
        /// Add Async
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
                return entity;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<T> AddAll(List<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            var result = _dbContext.SaveChanges();

            if (result > 0)
                return entities;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<List<T>> AddAllAsync(List<T> entities)
        {
            _dbContext.Set<T>().AddRange(entities);
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
                return entities;
            else
                return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public T Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            int result = _dbContext.SaveChanges();

            if (result > 0)
                return entity;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public List<T> UpdateAll(List<T> entities)
        {
            _dbContext.UpdateRange(entities);
            int result = _dbContext.SaveChanges();

            if (result > 0)
                return entities;
            else
                return null;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
                return entity;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="updatedEntity"></param>
        /// <returns></returns>
        public async Task<T> UpdateAsync(int id, T entity)
        {
            _dbContext.Entry<T>((await _dbContext.Set<T>().FindAsync(id))).CurrentValues.SetValues(entity);
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
                return entity;
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public async Task<List<T>> UpdateAllAsync(List<T> entities)
        {
            _dbContext.UpdateRange(entities);
            int result = await _dbContext.SaveChangesAsync();

            if (result > 0)
                return entities;
            else
                return null;
        }



        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }



        public void DeleteAll(List<T> entity)
        {
            _dbContext.Set<T>().RemoveRange(entity);
            _dbContext.SaveChanges();
        }

        public async Task DeleteAllAsync(List<T> entity)
        {
            _dbContext.Set<T>().RemoveRange(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            try
            {
                var query = _dbContext.Set<T>().Where(predicate);

                return query;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual IEnumerable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        private void TrySetProperty(object obj, string property, object value)
        {
            var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
                prop.SetValue(obj, value, null);
        }


    }
}
