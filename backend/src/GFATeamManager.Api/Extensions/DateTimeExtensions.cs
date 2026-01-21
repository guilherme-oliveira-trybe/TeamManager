namespace GFATeamManager.Api.Extensions;

public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo BrazilTimeZone = 
        TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");

    /// <summary>
    /// Converte DateTime UTC para o horário de Brasília (BRT/BRST)
    /// </summary>
    public static DateTime ToBrazilTime(this DateTime utcDateTime)
    {
        if (utcDateTime.Kind != DateTimeKind.Utc)
            throw new ArgumentException("DateTime must be in UTC", nameof(utcDateTime));
            
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, BrazilTimeZone);
    }

    /// <summary>
    /// Converte DateTime de Brasília (BRT/BRST) para UTC
    /// </summary>
    public static DateTime ToUtc(this DateTime brazilDateTime)
    {
        return TimeZoneInfo.ConvertTimeToUtc(brazilDateTime, BrazilTimeZone);
    }

    /// <summary>
    /// Retorna a segunda-feira da semana (00:00:00) em horário de Brasília
    /// </summary>
    public static DateTime GetWeekStartInBrazil(this DateTime utcDateTime)
    {
        var brazilNow = utcDateTime.ToBrazilTime();
        var dayOfWeek = (int)brazilNow.DayOfWeek;
        
        // Calcula dias desde segunda-feira
        var daysSinceMonday = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
        
        // Retorna segunda-feira 00:00:00 em horário de Brasília
        return brazilNow.Date.AddDays(-daysSinceMonday);
    }

    /// <summary>
    /// Retorna o domingo da semana (23:59:59.9999999) em horário de Brasília
    /// </summary>
    public static DateTime GetWeekEndInBrazil(this DateTime utcDateTime)
    {
        var weekStart = utcDateTime.GetWeekStartInBrazil();
        
        // Domingo 23:59:59.9999999
        return weekStart.AddDays(7).AddTicks(-1);
    }

    /// <summary>
    /// Normaliza data que vem do frontend (query params) sem timestamp.
    /// Interpreta como 00:00:00 em horário de Brasília e converte para UTC.
    /// Usado apenas em filtros GET, não em POST/PUT.
    /// </summary>
    public static DateTime NormalizeDateFromQuery(this DateTime dateTime)
    {
        // Se a hora é exatamente 00:00:00, interpreta como horário de Brasília
        if (dateTime.TimeOfDay == TimeSpan.Zero)
        {
            // Trata como data em horário de Brasília (ex: 2026-01-15 00:00:00 BRT)
            var brazilDate = DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
            return brazilDate.ToUtc();
        }
        
        // Caso contrário, assume que já é UTC
        return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
    }
}
