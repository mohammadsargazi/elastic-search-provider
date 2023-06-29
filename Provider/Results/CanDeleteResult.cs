namespace Provider.Results;

public record CanDeleteResult(bool CanDelete, string Message = "");
