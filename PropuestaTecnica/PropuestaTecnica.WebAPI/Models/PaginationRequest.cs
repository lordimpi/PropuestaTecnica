namespace PropuestaTecnica.WebAPI.Models;

public class PaginationRequest
{
    private const int MaxPageSize = 100;

    public int Page { get; set; } = 1;

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value <= 0) _pageSize = 10;
            else _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}