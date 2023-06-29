namespace Provider.Results;

public record UpdatedResult<T>(T entity, long updatedCount, string message);
