namespace GFATeamManager.Application.DTOS.User;

public class UserParameters
{
    private const int MaxPageSize = 50;
    public int? PageNumber { get; set; }
    private int _pageSize = 10;
    
    public int? PageSize
    {
        get => _pageSize;
        set 
        {
            if (value.HasValue)
            {
                _pageSize = (value.Value > MaxPageSize) ? MaxPageSize : value.Value;
            }
        }
    }

    public string? SearchTerm { get; set; }
    public int? Status { get; set; }
    
    public int GetActualPageNumber() => PageNumber ?? 1;
    public int GetActualPageSize() => _pageSize;
}
