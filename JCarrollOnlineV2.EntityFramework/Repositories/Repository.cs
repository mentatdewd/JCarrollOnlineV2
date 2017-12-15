using JCarrollOnlineV2.Repositories;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JCarrollOnlineV2.EntityFramework.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private JCarrollOnlineV2DbContext _dbContext;

        internal Repository(JCarrollOnlineV2DbContext dbContext) { _dbContext = dbContext; }

        internal DbSet<TEntity> Set
        {
            get { return _dbContext.Set<TEntity>(); }
        }

        public Repository(TEntity entity, DbContext context) { }

        public void Add(TEntity entity)
        {
            Set.Add(entity);
        }

        public TEntity FindById(object id)
        {
            return Set.Find(id);
        }

        public Task<TEntity> FindByIdAsync(object id)
        {
            return FindByIdAsync(id);
        }

        public Task<TEntity> FindByIdAsync(CancellationToken cancellationToken, object id)
        {
            return FindByIdAsync(cancellationToken, id);
        }

        public List<TEntity> GetAll()
        {
            return Set.ToList();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return Set.ToListAsync();
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return Set.ToListAsync(cancellationToken);
        }

        public List<TEntity> PageAll(int skip, int take)
        {
            return Set.Skip(skip).Take(take).ToList();
        }

        public Task<List<TEntity>> PageAllAsync(int skip, int take)
        {
            return Set.Skip(skip).Take(take).ToListAsync();
        }

        public Task<List<TEntity>> PageAllAsync(CancellationToken cancellationToken, int skip, int take)
        {
            return Set.Skip(skip).Take(take).ToListAsync(cancellationToken);
        }

        public void Remove(TEntity entity)
        {
            Set.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Set.RemoveRange(entities);
        }

        public int Save()
        {
            return _dbContext.SaveChanges();
        }

        public Task<int> SaveAsync()
        {
            return _dbContext.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            var entry = _dbContext.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                Set.Attach(entity);
                entry = _dbContext.Entry(entity);
            }

            entry.State = EntityState.Modified;
        }
    }
}
