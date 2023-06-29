namespace Provider.Contracts;

public interface IPagedRequest
{
    int Page { get; set; }
    int PageSize { get; set; }
    string? Sort { get; set; }
}
