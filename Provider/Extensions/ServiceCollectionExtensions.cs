using Microsoft.Extensions.DependencyInjection;
using Nest;
using Provider.Configuration;


namespace Provider.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureElasticSearch(this IServiceCollection services, ElasticConfiguration elasticConfiguration)
    {
        var uri = new Uri(elasticConfiguration.Uri);
        services.AddSingleton<IElasticClient>(
            new ElasticClient(
                new ConnectionSettings(uri)
                    .BasicAuthentication(elasticConfiguration.Username, elasticConfiguration.Password)
            ));

        return services;
    }
}