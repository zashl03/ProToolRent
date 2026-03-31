
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }

    public PagedResult(List<T> items, int totalCount)
    {
        Items = items;
        TotalCount = totalCount;
    }
}