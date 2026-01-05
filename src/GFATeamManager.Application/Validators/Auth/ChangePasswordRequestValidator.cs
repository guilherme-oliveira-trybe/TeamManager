using FluentValidation;
using GFATeamManager.Application.DTOS.Auth;

namespace GFATeamManager.Application.Validators.Auth;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória")
            .MinimumLength(8).WithMessage("Nova senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("Nova senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[0-9]").WithMessage("Nova senha deve conter pelo menos um número");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.NewPassword).WithMessage("As senhas não coincidem");
    }
}