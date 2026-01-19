using FluentValidation;
using GFATeamManager.Application.DTOS.Sector;

namespace GFATeamManager.Application.Validators.Sector;

public class UpdateSectorRequestValidator : AbstractValidator<UpdateSectorRequest>
{
    public UpdateSectorRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome deve ter no máximo 100 caracteres");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Descrição deve ter no máximo 500 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
