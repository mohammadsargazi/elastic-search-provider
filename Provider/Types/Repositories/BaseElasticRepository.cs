using Elasticsearch.Net;
using Microsoft.Extensions.Localization;
using Nest;
using Provider.Contracts;
using Provider.Extensions;
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

    public async Task<DeletedResult> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var can = await CanDelete(entity, cancellationToken).ConfigureAwait(false);

        if (!can.CanDelete) throw new InvalidOperationException(can.Message);

        var response = await ElasticClient.UpdateAsync<T>(new DocumentPath<T>(entity.Id), u => u
          .Index(GetIndexName())
          .Script(s => s
              .Source("if (ctx._source.version == params.version) { ctx.op = 'delete' }")
              .Params(p => p.Add("version", entity.Version))
              ));

        if (response.Result == Result.Updated) throw new InvalidOperationException(_localizer["Concurrency exception"]);

        return new DeletedResult(can, response.IsValid ? 1 : 0);
    }

    public virtual async Task<InsertResult<T>> InsertAsync(T entity, CancellationToken cancellationToken)
    {
        var response = await ElasticClient.IndexAsync(entity, x => x.Index(GetIndexName()).Refresh(Refresh.WaitFor), cancellationToken).ConfigureAwait(false);
        return response.ToInsertResult(entity, _localizer);
    }

    public async Task<List<InsertResult<T>>> InsertAllAsync(List<T> entities, CancellationToken cancellationToken)
    {
        if (!entities.Any())
            throw new ArgumentNullException(nameof(entities));

        var response = await ElasticClient.BulkAsync(b => b
            .Index(GetIndexName())
            .CreateMany(entities), cancellationToken);

        return entities.Select(entity =>
        {
            var success = !response.ItemsWithErrors.Any(item => item.Id == entity.Id.ToString());
            return entity.ToInsertResult(success, _localizer);
        }).ToList();

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