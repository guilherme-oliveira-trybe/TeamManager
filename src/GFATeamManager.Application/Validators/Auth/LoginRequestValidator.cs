using FluentValidation;
using GFATeamManager.Application.DTOS.Auth;
using GFATeamManager.Application.Extensions;

namespace GFATeamManager.Application.Validators.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Login)
            .NotEmpty().WithMessage("Login é obrigatório")
            .Must(BeValidLoginFormat).WithMessage("Login deve ser um email válido ou CPF com 11 dígitos");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres");
    }

    private static bool BeValidLoginFormat(string login)
        => login.Contains("@") || login.IsValidCpf();
}