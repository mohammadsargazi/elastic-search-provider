using Provider.Results;

namespace Provider.Contracts;

public interface IBaseRepository
{
    Task<long> CountAsync();
}

public interface IBaseRepository<T> : IBaseRepository where T : IBaseEntity
{
    Task<CanDeleteResult> CanDelete(T entity, CancellationToken cancellationToken);
    Task<DeletedResult> DeleteAsync(T entity, CancellationToken cancellationToken);
    Task<InsertResult<T>> InsertAsync(T entity, CancellationToken cancellationToken);
    Task<List<InsertResult<T>>> InsertAllAsync(List<T> entities, CancellationToken cancellationToken);
    Task<UpdatedResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<List<UpdatedResult<T>>> UpdateAllAsync(List<T> entities, CancellationToken cancellationToken);
    Task<IncrementResult> IncrementAsync(Guid id, string field, Guid version, double amount = 1);
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken);
    Task<T?> SingleOrDefaultAsync(Guid id, CancellationToken cancellationToken);
    Task<T> SingleAsync(Guid id, CancellationToken cancellationToken);
    Task<List<T>> ListAsync(CancellationToken cancellationToken);
    Task<List<T>> ListAsync(List<Guid?> ids, CancellationToken cancellationToken);
    Task<List<T>> ListAsync(List<Guid> ids, CancellationToken cancellationToken);
    Task<IPagedData<T>> ListPagedAsync(IPagedRequest request, CancellationToken cancellationToken);
}
