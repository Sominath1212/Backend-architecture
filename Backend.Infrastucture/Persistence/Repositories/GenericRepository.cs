using Backend.Application.Common.Models;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;
using Backend.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Backend.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;

    protected readonly DbSet<T> _dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // --------------------------------------------------
    // Get By Id
    // --------------------------------------------------

    public async Task<T?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FindAsync(
                new object[] { id },
                cancellationToken);
    }

    // --------------------------------------------------
    // First Or Default
    // --------------------------------------------------

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(
                predicate,
                cancellationToken);
    }

    // --------------------------------------------------
    // Get All
    // --------------------------------------------------

    public async Task<IReadOnlyList<T>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    // --------------------------------------------------
    // Find
    // --------------------------------------------------

    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    // --------------------------------------------------
    // Pagination
    // --------------------------------------------------

    public async Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        CancellationToken cancellationToken = default)
    {
        return await GetPagedAsync(
            request,
            null,
            cancellationToken);
    }

    public async Task<PagedResult<T>> GetPagedAsync(
        PagedRequest request,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet.AsNoTracking();

        if (predicate is not null)
        {
            query = query.Where(predicate);
        }

        var totalCount = await query
            .CountAsync(cancellationToken);

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }

    // --------------------------------------------------
    // Count
    // --------------------------------------------------

    public async Task<int> CountAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .CountAsync(
                predicate,
                cancellationToken);
    }

    // --------------------------------------------------
    // Any
    // --------------------------------------------------

    public async Task<bool> AnyAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(
                predicate,
                cancellationToken);
    }

    // --------------------------------------------------
    // Add
    // --------------------------------------------------

    public async Task AddAsync(
        T entity,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(
            entity,
            cancellationToken);
    }

    // --------------------------------------------------
    // Add Range
    // --------------------------------------------------

    public async Task AddRangeAsync(
        IEnumerable<T> entities,
        CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(
            entities,
            cancellationToken);
    }

    // --------------------------------------------------
    // Update
    // --------------------------------------------------

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    // --------------------------------------------------
    // Update Range
    // --------------------------------------------------

    public void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    // --------------------------------------------------
    // Delete
    // --------------------------------------------------

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    // --------------------------------------------------
    // Delete Range
    // --------------------------------------------------

    public void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
}
