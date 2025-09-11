using InDream.Common.BaseModels;
using InDream.Data;
using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : EntityBase
{
    private readonly DataContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(DataContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IQueryable<T> Table => _dbSet;

    public async Task<T> Single(long id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IList<T>> ListAll()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task Create(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

}