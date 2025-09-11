using InDream.Common.BaseModels;
using System.Linq.Expressions;

public interface IRepository<T> where T : EntityBase
{
    IQueryable<T> Table { get; }


    Task<T> Single(long id);
    Task<IList<T>> ListAll();
    Task Create(T entity);
    Task Update(T entity);
    Task Delete(T entity);
}