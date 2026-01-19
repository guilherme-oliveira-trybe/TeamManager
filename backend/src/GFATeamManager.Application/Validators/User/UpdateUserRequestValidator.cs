using FluentValidation;
using GFATeamManager.Application.DTOS.User;

namespace GFATeamManager.Application.Validators.User;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Nome completo é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Peso deve ser maior que zero")
            .LessThan(300).WithMessage("Peso deve ser menor que 300kg");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Altura deve ser maior que zero")
            .LessThan(250).WithMessage("Altura deve ser menor que 250cm");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MinimumLength(10).WithMessage("Telefone inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        When(x => !string.IsNullOrEmpty(x.EmergencyContactName), () =>
        {
            RuleFor(x => x.EmergencyContactName)
                .MinimumLength(3).WithMessage("Nome do contato deve ter no mínimo 3 caracteres");
        });

        When(x => !string.IsNullOrEmpty(x.EmergencyContactPhone), () =>
        {
            RuleFor(x => x.EmergencyContactPhone)
                .MinimumLength(10).WithMessage("Telefone do contato inválido");
        });
    }
}