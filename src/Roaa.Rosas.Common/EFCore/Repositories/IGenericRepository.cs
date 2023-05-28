using System.Linq.Expressions;

namespace Roaa.Rosas.Common.EFCore.Repositories
{
    public interface IGenericRepository<TDbContext, TEntity, TId> where TEntity : class
                                                                  where TDbContext : IDbContext
    {

        #region Props 
        //public TDbContext DbContext { get;  }  

        #endregion


        #region Collection

        #region async  
        Task<List<TEntity>> GetAllAsync();

        Task<List<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> match);

        Task<List<TEntity>> FindByAsync(Expression<Func<TEntity, bool>> predicate);

        #endregion

        #region sync    
        List<TEntity> GetAll();

        List<TEntity> FindAll(Expression<Func<TEntity, bool>> match);

        List<TEntity> FindBy(Expression<Func<TEntity, bool>> predicate);
        #endregion

        #endregion


        #region Queryable        
        IQueryable<TEntity> GetAllAsQueryable();

        IQueryable<TEntity> FindAsQueryableBy(Expression<Func<TEntity, bool>> predicate);

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] includeProperties);

        #endregion


        #region Item

        #region async   
        Task<TEntity> GetAsync(TId id);

        Task<TEntity> FindAsync(Expression<Func<TEntity, bool>> match);

        #endregion

        #region sync
        TEntity Get(TId id);

        TEntity Find(Expression<Func<TEntity, bool>> match);
        #endregion

        #endregion


        #region  Others

        #region  async     
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync();

        #endregion

        #region  sync     
        bool Any(Expression<Func<TEntity, bool>> predicate);


        int Count();
        #endregion

        #endregion


        #region Create / Update / Delete / Save 

        #region async
        Task<bool> AddAsync(TEntity entity);

        Task<int> DeleteAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity, object key);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> SaveAsync();

        #endregion

        #region  sync
        TEntity Add(TEntity entity);

        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entities);


        void Delete(TEntity entity);

        void Update(TEntity entity, object key);

        void Update(TEntity entity);

        bool Save();
        #endregion

        #endregion

        void Dispose();


    }
}
