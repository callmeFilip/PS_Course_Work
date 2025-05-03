using AccessControlSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AccessControlSystem.Data.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> GetByIdAsync(int id);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Remove(TEntity entity);
    }

    internal class GenericRepository<TEntity> : IGenericRepository<TEntity>
                                                where TEntity : class
    {
        protected readonly AccessControlContext _ctx;
        internal GenericRepository(AccessControlContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<TEntity>> GetAllAsync() =>
            await _ctx.Set<TEntity>().AsNoTracking().ToListAsync();

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> p) =>
            await _ctx.Set<TEntity>().AsNoTracking().Where(p).ToListAsync();

        public async Task<TEntity?> GetByIdAsync(int id) =>
            await _ctx.Set<TEntity>().FindAsync(id);

        public async Task AddAsync(TEntity entity) =>
            await _ctx.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity) => _ctx.Set<TEntity>().Update(entity);
        public void Remove(TEntity entity) => _ctx.Set<TEntity>().Remove(entity);
    }
}
