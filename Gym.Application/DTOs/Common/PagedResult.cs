namespace Gym.Application.DTOs.Common;

public class PagedResult<T>
{
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();

}