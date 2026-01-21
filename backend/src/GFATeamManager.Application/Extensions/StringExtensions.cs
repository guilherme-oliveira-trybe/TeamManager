namespace GFATeamManager.Application.Extensions;

public static class StringExtensions
{
    public static bool IsValidCpf(this string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        // Remove caracteres não numéricos
        var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

        // CPF deve ter exatamente 11 dígitos
        if (cleanCpf.Length != 11)
            return false;

        // Rejeita CPFs com todos os dígitos iguais
        if (cleanCpf.Distinct().Count() == 1)
            return false;

        // Valida primeiro dígito verificador
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += int.Parse(cleanCpf[i].ToString()) * (10 - i);

        var remainder = sum % 11;
        var digit1 = remainder < 2 ? 0 : 11 - remainder;

        if (int.Parse(cleanCpf[9].ToString()) != digit1)
            return false;

        // Valida segundo dígito verificador
        sum = 0;
        for (var i = 0; i < 10; i++)
            sum += int.Parse(cleanCpf[i].ToString()) * (11 - i);

        remainder = sum % 11;
        var digit2 = remainder < 2 ? 0 : 11 - remainder;

        return int.Parse(cleanCpf[10].ToString()) == digit2;
    }

    public static string CleanCpf(this string cpf)
    {
        return new string(cpf.Where(char.IsDigit).ToArray());
    }
}