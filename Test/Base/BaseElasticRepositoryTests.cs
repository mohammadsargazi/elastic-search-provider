namespace Test.Base;

public class BaseElasticRepositoryTests<T, TRepository>
    where T : BaseElasticEntity
    where TRepository : BaseElasticRepository<T>
{
    private Mock<IStringLocalizer<BaseElasticRepository<T>>> _localizerMock;
    protected TRepository _repository;

    public BaseElasticRepositoryTests()
    {
        var elasticConfiguration = new ElasticConfiguration
        {
            Uri = "http://localhost:9200/"
        };

        var connectionSettings = new ConnectionSettings(new Uri(elasticConfiguration.Uri));

        var elasticClient = new ElasticClient(connectionSettings);

        _localizerMock = new Mock<IStringLocalizer<BaseElasticRepository<T>>>();
        _repository = (TRepository)Activator.CreateInstance(typeof(TRepository), elasticClient, _localizerMock.Object);
    }

    protected T GenerateSampleEntity(Guid id)
    {
        var entity = default(T);

        var constructor = typeof(T).GetConstructor(new[] { typeof(string) });
        if (constructor != null)
        {
            entity = (T)constructor.Invoke(new object[] { "value" });
        }
        else
        {
            entity = (T)Activator.CreateInstance(typeof(T));
        }

        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty != null && idProperty.PropertyType == typeof(Guid))
        {
            idProperty.SetValue(entity, id);
        }

        var versionProperty = typeof(T).GetProperty("Version");
        if (versionProperty != null && versionProperty.PropertyType == typeof(Guid))
        {
            versionProperty.SetValue(entity, Guid.NewGuid());
        }

        return entity;
    }
}
