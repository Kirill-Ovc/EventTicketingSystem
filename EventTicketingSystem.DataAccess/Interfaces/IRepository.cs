namespace EventTicketingSystem.DataAccess.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> Find(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(int id);
        Task SaveChanges();
    }
}
