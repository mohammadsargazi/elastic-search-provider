using Microsoft.Extensions.Localization;
using Nest;
using Provider.Contracts;
using Provider.Results;
using Provider.Types.Entities;

namespace Provider.Types.Repositories;

public abstract class BaseElasticRepository<T> : IBaseRepository<T> where T : BaseElasticEntity
{
    protected IElasticClient ElasticClient { get; }
    private string _indexName;
    private readonly IStringLocalizer<BaseElasticRepository<T>> _localizer;

    protected BaseElasticRepository(IElasticClient elasticClient, IStringLocalizer<BaseElasticRepository<T>> localizer)
    {
        ElasticClient = elasticClient;
        _localizer = localizer;
        _indexName = typeof(T).ToString().ToLower();
    }

    public string GetIndexName()
    {
        return _indexName;
    }

    public abstract Task<CanDeleteResult> CanDelete(T entity, CancellationToken cancellationToken);

    public Task<DeletedResult> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<InsertResult<T>> InsertAsync(T entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<InsertResult<T>>> InsertAllAsync(List<T> entities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<UpdatedResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<UpdatedResult<T>>> UpdateAllAsync(List<T> entities, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IncrementResult> IncrementAsync(Guid id, string field, Guid version, double amount = 1)
    {
        throw new NotImplementedException();
    }

    public Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<T?> SingleOrDefaultAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<T> SingleAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> ListAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> ListAsync(List<Guid?> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<List<T>> ListAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IPagedData<T>> ListPagedAsync(IPagedRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync()
    {
        throw new NotImplementedException();
    }
}