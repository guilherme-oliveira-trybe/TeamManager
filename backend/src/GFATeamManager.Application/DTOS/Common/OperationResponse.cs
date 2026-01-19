namespace GFATeamManager.Application.DTOS.Common;

public class OperationResponse
{
    public List<string> Errors { get; set; } = new();

    public bool IsSuccess => Errors.Count == 0;

    public static OperationResponse Success()
    {
        return new OperationResponse();
    }

    public static OperationResponse Failure(params string[] errors)
    {
        return new OperationResponse
        {
            Errors = errors.ToList()
        };
    }

    public static OperationResponse Failure(List<string> errors)
    {
        return new OperationResponse
        {
            Errors = errors
        };
    }
}