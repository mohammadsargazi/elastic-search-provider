using Provider.Contracts;

public class PagedData<T> : IPagedData<T>
{
    public PagedData(List<T> result, long totalCount)
    {
        Result = result;
        TotalCount = totalCount;
    }
    public List<T> Result { get; set; }
    public long TotalCount { get; set; }
}