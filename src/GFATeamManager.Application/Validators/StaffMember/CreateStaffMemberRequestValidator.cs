using FluentValidation;
using GFATeamManager.Application.DTOS.StaffMember;

namespace GFATeamManager.Application.Validators.StaffMember;

public class CreateStaffMemberRequestValidator : AbstractValidator<CreateStaffMemberRequest>
{
    public CreateStaffMemberRequestValidator()
    {
        RuleFor(x => x.SectorId)
            .NotEmpty().WithMessage("Setor é obrigatório");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Nome completo é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Email inválido")
            .MaximumLength(200).WithMessage("Email deve ter no máximo 200 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MinimumLength(10).WithMessage("Telefone deve ter no mínimo 10 dígitos")
            .MaximumLength(20).WithMessage("Telefone deve ter no máximo 20 caracteres");

        RuleFor(x => x.Specialty)
            .MaximumLength(100).WithMessage("Especialidade deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrWhiteSpace(x.Specialty));
    }
}
