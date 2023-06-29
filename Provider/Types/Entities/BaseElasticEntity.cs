using Provider.Contracts;

namespace Provider.Types.Entities;

public record BaseElasticEntity(Guid Id, DateTimeOffset CreateOn, DateTimeOffset? UpdateOn, Guid Version) : IBaseEntity
{
    protected BaseElasticEntity() : this(Guid.NewGuid(), DateTimeOffset.Now, null, Guid.NewGuid()) { }
}