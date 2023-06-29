namespace Provider.Results;

public record DeletedResult(CanDeleteResult CanDeleteResult, long DeletedCount = 0) : CanDeleteResult(CanDeleteResult.CanDelete, CanDeleteResult.Message);
