using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;

namespace EventTicketingSystem.DataAccess.Services
{
    internal abstract class BaseRepository<TEntity> : IRepository<TEntity> 
        where TEntity : class, IEntity
    {
        private readonly DatabaseContext _context;

        protected BaseRepository(DatabaseContext context)
        {
            _context = context;
        }

        public virtual async Task<TEntity> Find(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public virtual async Task Add(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public virtual async Task Update(TEntity entity)
        {
            var found = await _context.Set<TEntity>().FindAsync(entity.Id);
            if (found is null)
            {
                throw new InvalidOperationException("Update failed. Can't find entity by Id");
            }
            _context.Set<TEntity>().Update(entity);
        }

        public virtual async Task Delete(int id)
        {
            var entity = await _context.Set<TEntity>().FindAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}
