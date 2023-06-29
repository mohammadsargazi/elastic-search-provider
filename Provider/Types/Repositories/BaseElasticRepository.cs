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

    public async Task<UpdatedResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        var oldVersion = entity.Version;
        entity = entity with { UpdateOn = DateTimeOffset.Now, Version = Guid.NewGuid() };

        var response = await ElasticClient.UpdateAsync<T>(new DocumentPath<T>(entity.Id), u => u
            .Index(GetIndexName())
            .Script(s => s
                .Source($"if (ctx._source.version == '{oldVersion}') {{ ctx._source = params.newEntity; }} else {{ throw new Exception('Invalid version'); }}")
                .Params(p => p.Add("newEntity", entity))));

        if (response.Result == Result.Error) throw new InvalidOperationException(_localizer["Concurrency exception"]);

        return entity.ToUpdateResult(response.Result, _localizer);
    }

    public async Task<List<UpdatedResult<T>>> UpdateAllAsync(List<T> entities, CancellationToken cancellationToken)
    {
        var bulkDescriptor = new BulkDescriptor();
        var versionDictionary = new Dictionary<Guid, Guid>();

        foreach (var entity in entities)
        {
            var oldVersion = entity.Version;
            var updatedEntity = entity with { UpdateOn = DateTimeOffset.Now, Version = Guid.NewGuid() };

            versionDictionary.Add(entity.Id, entity.Version);

            bulkDescriptor.Update<T>(u => u
                .Id(entity.Id)
                .Index(GetIndexName())
                .Script(s => s
                    .Source($"if (ctx._source.version == '{oldVersion}') {{ ctx._source = params.newEntity; }} else {{ throw new Exception('Invalid version'); }}")
                    .Params(p => p.Add("newEntity", updatedEntity))
                ));
        }

        var bulkResponse = await ElasticClient.BulkAsync(bulkDescriptor);

        if (bulkResponse.Errors) throw new InvalidOperationException(_localizer["Concurrency exception"]);

        return entities.Select(entity =>
        {
            var success = !bulkResponse.ItemsWithErrors.Any(item => item.Id == entity.Id.ToString());
            return entity.ToUpdateResult(success, _localizer);
        }).ToList();
    }

    public async Task<IncrementResult> IncrementAsync(Guid id, string field, Guid version, double amount = 1)
    {
        var response = await ElasticClient.UpdateAsync<T>(id, u => u
        .Index(GetIndexName())
        .Script(s => s
        .Source($"if (ctx._source.version == '{version}')  {{ctx._source.{field} += params.amount }}  else {{ throw new Exception('Invalid version');  }}")
        .Params(p => p.Add("amount", amount))));

        if (response.Result == Result.Error) throw new InvalidOperationException(_localizer["Concurrency exception"]);

        return new IncrementResult(response.IsValid);
    }

    public async Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken)
    {
        var response = await ElasticClient.SearchAsync<T>(u => u.Index(GetIndexName()).Query(q => q.MatchAll()).Size(1).TrackTotalHits(false), cancellationToken);
        return response.Documents.FirstOrDefault();
    }

    public async Task<T?> SingleOrDefaultAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await ElasticClient.GetAsync(new DocumentPath<T>(
            new Id(id)), x => x.Index(GetIndexName()), cancellationToken);
        return response?.Source;
    }

    public async Task<T> SingleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await ElasticClient.GetAsync<T>(id, g => g.Index(GetIndexName()), cancellationToken);

        return response.Source;
    }

    public async Task<List<T>> ListAsync(CancellationToken cancellationToken)
    {
        var response = await ElasticClient.SearchAsync<T>(u => u.Index(GetIndexName()).Query(q => q.MatchAll()), cancellationToken);
        return response.Documents.ToList();
    }

    public async Task<List<T>> ListAsync(List<Guid?> ids, CancellationToken cancellationToken)
    {
        var response = await ElasticClient.SearchAsync<T>(s => s
        .Index(GetIndexName())
        .Query(q => q.Terms(t => t.Field(f => f.Id).Terms(ids.Where(id => id.HasValue).Select(id => id.Value))))
        .Size(ids.Count)
        .RequestConfiguration(r => r.ThrowExceptions()));

        if (!response.IsValid) throw new Exception($"Error occurred while searching: {response.ServerError}");

        return response.Documents.ToList();
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