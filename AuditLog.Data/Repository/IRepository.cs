using System.Linq.Expressions;

namespace AuditLog.Data
{
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : class
    {

        int Count();
        int Count(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync();
        void Delete(TEntity entity);
        void DeleteRange(List<TEntity> entities);
        void Delete(TPrimaryKey id);
        void Delete(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(TPrimaryKey id);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
        Task DeleteAsync(TEntity entity);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefaultWithWhere(Expression<Func<TEntity, bool>> predicate);
        TEntity FirstOrDefault(TPrimaryKey id);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
        TEntity Get(TPrimaryKey id);
        IQueryable<TEntity> GetAll();
        List<TEntity> GetAllIncluding(Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null);

        IQueryable<TEntity> GetAllIncluding(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IQueryable<TEntity>> includeMembers = null);
        TEntity GetIncludingByIdAsyn(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>,
            IQueryable<TEntity>> includeMembers = null);
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        List<TEntity> GetAllList();
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllListAsync();
        Task<TEntity> GetAsync(TPrimaryKey id);
        TEntity Insert(TEntity entity);        
        List<TEntity> InsertRange(List<TEntity> entities);

        Task<List<TEntity>> InsertRangeAsyn(List<TEntity> entities);
        Task<TEntity> InsertAsync(TEntity entity);
        
        TEntity Load(TPrimaryKey id);
        long LongCount(Expression<Func<TEntity, bool>> predicate);
        long LongCount();
        Task<long> LongCountAsync();
        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Update(TEntity entity);
        
        Task<TEntity> UpdateAsync(TEntity entity);
        EmployeeDbContext GetContext();
        void SaveChanges();

        string GetOpenConnection();
        
    }
}
