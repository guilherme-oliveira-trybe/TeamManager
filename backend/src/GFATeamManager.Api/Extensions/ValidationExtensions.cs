using FluentValidation;
using GFATeamManager.Application.DTOS.Common;

namespace GFATeamManager.Api.Extensions;

public static class ValidationExtensions
{
    public static async Task<(bool IsValid, BaseResponse<T>? ErrorResponse)> ValidateRequest<T, TRequest>(
        this IValidator<TRequest> validator, 
        TRequest request) where T : class
    {
        var validationResult = await validator.ValidateAsync(request);
        
        if (validationResult.IsValid)
            return (true, null);
        
        var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return (false, BaseResponse<T>.Failure(errors));
    }
}