namespace GFATeamManager.Application.DTOS.Common;

public class BaseResponse<T>
{
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = [];
    
    public bool IsSuccess => Errors.Count == 0;
    
    public static BaseResponse<T> Success(T data)
    {
        return new BaseResponse<T>
        {
            Data = data
        };
    }
    
    public static BaseResponse<T> Failure(params string[] errors)
    {
        return new BaseResponse<T>
        {
            Errors = errors.ToList()
        };
    }

    public static BaseResponse<T> Failure(List<string> errors)
    {
        return new BaseResponse<T>
        {
            Errors = errors
        };
    }
}