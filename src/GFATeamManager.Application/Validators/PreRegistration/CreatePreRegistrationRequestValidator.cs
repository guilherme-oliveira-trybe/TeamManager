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
    }
}