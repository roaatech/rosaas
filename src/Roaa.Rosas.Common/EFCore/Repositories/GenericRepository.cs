using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Roaa.Rosas.Common.EFCore.Repositories
{
    public abstract class GenericRepository<TDbContext, TEntity, TKey> : IGenericRepository<TDbContext, TEntity, TKey>
                                                                          where TEntity : class
                                                                          where TDbContext : DbContext, IDbContext
    {
        #region Props  

        public TDbContext DbContext { get; private set; }
        protected DbSet<TEntity> DbSet { get; private set; }

        #endregion

        #region Ctors
        public GenericRepository(TDbContext context)
        {
            DbContext = context;
            DbSet = context.Set<TEntity>();
        }
        #endregion






        #region Pagination


        #endregion


        #region Collection

        #region async  
        public virtual async Task<List<TEntity>> GetAllAsync()
        {

            return await DbSet.ToListAsync();
        }

        public virtual async Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match)
        {
            return await DbSet.Where(match).ToListAsync();
        }

        public virtual async Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        #endregion

        #region sync    
        public virtual List<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual List<TEntity> FindAll(Expression<Func<TEntity, bool>> match)
        {
            return DbSet.Where(match).ToList();
        }

        public virtual List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate).ToList();
        }
        #endregion

        #endregion


        #region Queryable        
        public virtual IQueryable<TEntity> GetAllAsQueryable()
        {
            return DbSet;
        }

        public virtual IQueryable<TEntity> FindAsQueryableBy(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = DbSet.Where(predicate);
            return query;
        }

        public virtual IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties)
        {

            IQueryable<TEntity> queryable = GetAllAsQueryable();
            foreach (Expression<Func<TEntity, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include<TEntity, object>(includeProperty);
            }

            return queryable;
        }

        #endregion


        #region Item

        #region async   
        public virtual async Task<TEntity> GetAsync(TKey id)
        {
            return await DbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match)
        {
            return await DbSet.SingleOrDefaultAsync(match);
        }

        #endregion

        #region sync
        public virtual TEntity Get(TKey id)
        {
            return DbSet.Find(id);
        }

        public virtual TEntity Find(Expression<Func<TEntity, bool>> match)
        {
            return DbSet.SingleOrDefault(match);
        }
        #endregion

        #endregion


        #region  Others

        #region  async     
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var any = await DbSet.AnyAsync(predicate);
            return any;
        }


        public virtual async Task<int> CountAsync()
        {
            return await DbSet.CountAsync();
        }

        #endregion     

        #region  sync     
        public virtual bool Any(Expression<Func<TEntity, bool>> predicate)
        {
            bool any = DbSet.Any(predicate);
            return any;
        }


        public virtual int Count()
        {
            return DbSet.Count();
        }
        #endregion

        #endregion


        #region Create / Update / Delete / Save 

        #region async
        public virtual async Task<bool> AddAsync(TEntity t)
        {
            DbSet.Add(t);
            var result = await DbContext.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        public virtual async Task<int> DeleteAsync(TEntity entity)
        {
            DbSet.Remove(entity);
            return await DbContext.SaveChangesAsync();
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity t, object key)
        {
            if (t == null)
                return null;
            TEntity exist = await DbSet.FindAsync(key);
            if (exist != null)
            {
                DbContext.Entry(exist).CurrentValues.SetValues(t);
                await DbContext.SaveChangesAsync();
            }
            return exist;
        }

        public virtual async Task<bool> UpdateAsync(TEntity entity)
        {
            DbContext.Entry(entity).State = EntityState.Modified;
            var res = (await DbContext.SaveChangesAsync() >= 0);
            return res;
        }

        public virtual async Task<bool> SaveAsync()
        {
            var result = await DbContext.SaveChangesAsync();
            return result > 0 ? true : false;
        }

        #endregion

        #region  sync
        public virtual TEntity Add(TEntity t)
        {

            DbSet.Add(t);
            return t;
        }

        public virtual IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities)
        {
            DbSet.AddRange(entities);
            return entities;
        }


        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void Update(TEntity t, object key)
        {

            TEntity exist = DbSet.Find(key);
            DbContext.Entry(exist).CurrentValues.SetValues(t);

        }

        public virtual void Update(TEntity t)
        {
            DbContext.Entry(t).State = EntityState.Modified;
        }

        public virtual bool Save()
        {
            var result = DbContext.SaveChanges();
            return result > 0 ? true : false;
        }
        #endregion

        #endregion
















        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    DbContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
