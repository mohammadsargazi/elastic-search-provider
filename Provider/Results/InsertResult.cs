namespace Provider.Results;

public record InsertResult<T>(T entity, long InsertedCount, string message);
