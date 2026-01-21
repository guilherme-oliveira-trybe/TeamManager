using FluentValidation;
using GFATeamManager.Application.DTOS.PreRegistration;
using GFATeamManager.Application.Extensions;

namespace GFATeamManager.Application.Validators.PreRegistration;

public class CreatePreRegistrationRequestValidator : AbstractValidator<CreatePreRegistrationRequest>
{
    public CreatePreRegistrationRequestValidator()
    {
        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(cpf => cpf.IsValidCpf()).WithMessage("CPF inválido");

        RuleFor(x => x.Profile)
            .IsInEnum().WithMessage("Perfil inválido");

        RuleFor(x => x.Unit)
            .IsInEnum().When(x => x.Unit.HasValue).WithMessage("Unidade inválida");

        RuleFor(x => x.Position)
            .IsInEnum().When(x => x.Position.HasValue).WithMessage("Posição inválida");
    }
}