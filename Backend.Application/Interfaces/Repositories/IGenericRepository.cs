using Backend.Application.Common.Models;
using Backend.Domain.Common;
using System.Linq.Expressions;

namespace Backend.Application.Interfaces.Repositories;

public interface IGenericRepository<T>
    where T : BaseEntity
{
    // -------------------------
    // Get
    // -------------------------

    Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // -------------------------
    // Pagination
    // -------------------------

    Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default);

    Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    // -------------------------
    // Count / Exists
    // -------------------------

    Task<int> CountAsync(
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    // -------------------------
    // CRUD
    // -------------------------

    Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default);

    void Update(T entity);

    void UpdateRange(IEnumerable<T> entities);

    void Delete(T entity);

    void DeleteRange(IEnumerable<T> entities);
}
