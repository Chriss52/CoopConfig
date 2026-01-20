namespace CoopCloud.GeneralSettingsAndAuth.Features.Shared.Application.Models;

public class PaginatedResult<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}

public class PaginationParams
{
    private int _page = 1;
    private int _pageSize = 10;
    private const int MaxPageSize = 100;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 10 : value;
    }

    public string? Search { get; set; }
}
