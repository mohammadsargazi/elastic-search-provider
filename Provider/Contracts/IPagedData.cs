namespace Provider.Contracts;

public interface IPagedData<T>
{
    List<T> Result { get; set; }

    long TotalCount { get; set; }
}
