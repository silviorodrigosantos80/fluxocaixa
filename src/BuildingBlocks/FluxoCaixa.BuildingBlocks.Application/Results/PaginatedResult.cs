namespace FluxoCaixa.BuildingBlocks.Application.Results;

public sealed class PaginatedResult<T> : Result<IEnumerable<T>>
{
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    private PaginatedResult(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalCount)
        : base(items)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    public static PaginatedResult<T> Success(
        IEnumerable<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new PaginatedResult<T>(items, page, pageSize, totalCount);
    }

    public static new PaginatedResult<T> Failure(Error error)
    {
        return new PaginatedResult<T>(
            Enumerable.Empty<T>(),
            0,
            0,
            0)
        {
        };
    }
}