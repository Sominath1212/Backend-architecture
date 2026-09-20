using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;

namespace Backend.Application.Interfaces
{
public interface IUnitOfWork
{
    IGenericRepository<T> Repository<T>()
        where T : BaseEntity;

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
}