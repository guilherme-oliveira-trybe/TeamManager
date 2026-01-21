using FluentValidation;
using GFATeamManager.Application.DTOS.User;
using GFATeamManager.Application.Extensions;

namespace GFATeamManager.Application.Validators.User;

public class CompleteRegistrationRequestValidator : AbstractValidator<CompleteRegistrationRequest>
{
    public CompleteRegistrationRequestValidator()
    {
        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF é obrigatório")
            .Must(cpf => cpf.IsValidCpf()).WithMessage("CPF inválido");

        RuleFor(x => x.ActivationCode)
            .NotEmpty().WithMessage("Código de ativação é obrigatório")
            .Length(8).WithMessage("Código deve ter 8 caracteres");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Nome completo é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("Data de nascimento é obrigatória")
            .Must(BeValidAge).WithMessage("Usuário deve ter pelo menos 13 anos");

        RuleFor(x => x.Weight)
            .GreaterThan(0).WithMessage("Peso deve ser maior que zero")
            .LessThan(300).WithMessage("Peso deve ser menor que 300kg");

        RuleFor(x => x.Height)
            .GreaterThan(0).WithMessage("Altura deve ser maior que zero")
            .LessThan(250).WithMessage("Altura deve ser menor que 250cm");

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefone é obrigatório")
            .MinimumLength(11).WithMessage("Telefone inválido");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(8).WithMessage("Senha deve ter no mínimo 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("Senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[0-9]").WithMessage("Senha deve conter pelo menos um número");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Confirmação de senha é obrigatória")
            .Equal(x => x.Password).WithMessage("As senhas não coincidem");

        RuleFor(x => x.EmergencyContactName)
            .NotEmpty().WithMessage("Nome do contato de emergência é obrigatório")
            .MinimumLength(3).WithMessage("Nome do contato deve ter no mínimo 3 caracteres");

        RuleFor(x => x.EmergencyContactPhone)
            .NotEmpty().WithMessage("Telefone do contato de emergência é obrigatório")
            .MinimumLength(11).WithMessage("Telefone do contato inválido");
    }

    private static bool BeValidAge(DateTime birthDate)
    {
        var age = DateTime.Today.Year - birthDate.Year;
        if (birthDate.Date > DateTime.Today.AddYears(-age)) age--;
        return age >= 13;
    }
}