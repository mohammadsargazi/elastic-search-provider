namespace Test.Repositories;

public class TestElasticRepository : BaseElasticRepository<TestEntity>
{
    public TestElasticRepository(IElasticClient elasticClient, IStringLocalizer<BaseElasticRepository<TestEntity>> localizer)
           : base(elasticClient, localizer)
    {
    }

    public override Task<CanDeleteResult> CanDelete(TestEntity entity, CancellationToken cancellationToken)
    {
        return Task.FromResult(new CanDeleteResult(true, "Deletion is allowed."));
    }
}
