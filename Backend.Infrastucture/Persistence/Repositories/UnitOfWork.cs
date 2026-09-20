using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;
using Backend.Infrastructure.Persistence.Context;

namespace Backend.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<T> Repository<T>()
        where T : BaseEntity
    {
        var type = typeof(T);

        if (!_repositories.TryGetValue(type, out var repository))
        {
            repository = new GenericRepository<T>(_context);
            _repositories[type] = repository;
        }

        return (IGenericRepository<T>)repository;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
