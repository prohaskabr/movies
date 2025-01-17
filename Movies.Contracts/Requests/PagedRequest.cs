namespace Movies.Contracts.Requests;

public class PagedRequest
{
    public int Page { get; init; } = 1;
    public int PagedSize { get; init; } = 10;
}
