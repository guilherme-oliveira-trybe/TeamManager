using FluentValidation;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.Extensions;

namespace GFATeamManager.Application.Validators.Auth;

public class RequestPasswordResetRequestValidator : AbstractValidator<RequestPasswordResetRequest>
{
    public RequestPasswordResetRequestValidator()
    {
        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(cpf => cpf.IsValidCpf()).WithMessage("CPF inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");
    }
}