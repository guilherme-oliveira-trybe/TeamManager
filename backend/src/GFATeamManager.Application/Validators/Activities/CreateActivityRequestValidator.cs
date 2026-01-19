using FluentValidation;
using GFATeamManager.Application.DTOS.Activities;

namespace GFATeamManager.Application.Validators.Activities;

public class CreateActivityRequestValidator : AbstractValidator<CreateActivityRequest>
{
    public CreateActivityRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Tipo de atividade inválido");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Data de início é obrigatória")
            .Must(BeAValidDate).WithMessage("Data de início inválida");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("Data de término é obrigatória")
            .Must(BeAValidDate).WithMessage("Data de término inválida")
            .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("Data de término deve ser maior ou igual à data de início");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Local é obrigatório")
            .MinimumLength(3).WithMessage("Local deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Local deve ter no máximo 200 caracteres");

        RuleFor(x => x.TargetUnit)
            .IsInEnum().When(x => x.TargetUnit.HasValue)
            .WithMessage("Unidade alvo inválida");
    }

    private static bool BeAValidDate(DateTime date)
    {
        return date != default && date.Year is >= 2020 and <= 2100;
    }
}
