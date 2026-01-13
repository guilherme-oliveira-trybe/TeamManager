using FluentValidation;
using GFATeamManager.Application.DTOS.Activities;

namespace GFATeamManager.Application.Validators.Activities;

public class CreateActivityItemRequestValidator : AbstractValidator<CreateActivityItemRequest>
{
    public CreateActivityItemRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Título é obrigatório")
            .MinimumLength(3).WithMessage("Título deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Título deve ter no máximo 100 caracteres");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Horário de início é obrigatório")
            .Must(BeAValidDateTime).WithMessage("Horário de início inválido");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Horário de término é obrigatório")
            .Must(BeAValidDateTime).WithMessage("Horário de término inválido")
            .GreaterThanOrEqualTo(x => x.StartTime)
                .WithMessage("Horário de término deve ser maior ou igual ao horário de início");

        RuleFor(x => x.TargetUnit)
            .IsInEnum().When(x => x.TargetUnit.HasValue)
            .WithMessage("Unidade alvo inválida");

        RuleForEach(x => x.TargetPositions)
            .IsInEnum().When(x => x.TargetPositions.Count != 0)
            .WithMessage("Posição alvo inválida na lista");
    }

    private static bool BeAValidDateTime(DateTime dateTime)
    {
        return dateTime != default && dateTime.Year is >= 2020 and <= 2100;
    }
}
