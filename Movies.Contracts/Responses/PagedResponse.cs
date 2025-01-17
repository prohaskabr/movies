namespace Movies.Contracts.Responses;

public class PagedResponse<TResponse>
{
    public required IEnumerable<TResponse> Items { get; init; } = Enumerable.Empty<TResponse>();

    public int PageSize { get; set; }
    public int Page { get; set; }
    public int Total { get; set; }
    public bool HasNextPage => Total > Page * PageSize;
}
