namespace Provider.Contracts;

public interface IBaseEntity
{
    Guid Id { get; init; }
    DateTimeOffset CreateOn { get; init; }
    DateTimeOffset? UpdateOn { get; init; }
    Guid Version { get; init; }
}

