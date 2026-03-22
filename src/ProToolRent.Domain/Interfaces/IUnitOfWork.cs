
namespace ProToolRent.Domain.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangeAsync(CancellationToken cancellationToken = default);
}
