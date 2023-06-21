
namespace Domain.Repository
{
    /*public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }*/


    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        int SaveChanges();
        void Dispose();
        Task<int> SaveChangesAsync();
    }
}
