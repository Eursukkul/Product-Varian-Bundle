using System.Linq.Expressions;

namespace FlowAccount.Domain.Interfaces;

/// <summary>
/// Generic Repository Interface สำหรับ CRUD operations พื้นฐาน
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task<T> AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Remove(T entity);

    void RemoveRange(IEnumerable<T> entities);

    Task<bool> ExistsAsync(int id);

    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
}
