using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;

namespace EventTicketingSystem.DataAccess.Services
{
    internal class BaseRepository<TEntity> : IRepository<TEntity> 
        where TEntity : class, IEntity
    {
        private readonly DatabaseContext _context;

        public BaseRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<TEntity> Find(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public async Task Add(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }

        public async Task Update(TEntity entity)
        {
            var found = await _context.Set<TEntity>().FindAsync(entity.Id);
            if (found == null)
            {
                throw new InvalidOperationException("Update failed. Can't find entity by Id");
            }
            _context.Set<TEntity>().Update(entity);
        }

        public async Task Delete(int id)
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
