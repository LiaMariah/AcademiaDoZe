
namespace AcademiaDoZe.Application.DTOs.ControleAcesso;

public class RegistroAcessoDto
{
    public string Id { get; set; } = string.Empty;
    public string PessoaId { get; set; } = string.Empty;
    public string NomePessoa { get; set; } = string.Empty;

    public TipoPessoaAcesso Tipo { get; set; }

    public DateTime Entrada { get; set; }
    public DateTime? Saida { get; set; }

    public bool EhSaida { get; set; }

    public string Horario =>
        (EhSaida ? Saida ?? Entrada : Entrada).ToString("HH:mm");

    public TimeSpan? TempoPermanencia =>
        EhSaida && Saida.HasValue
            ? Saida.Value - Entrada
            : null;

    public string TempoPermanenciaFormatado
    {
        get
        {
            if (TempoPermanencia is not TimeSpan tempo)
                return string.Empty;

            if (tempo.TotalSeconds < 60)
                return $"{Math.Max(0, (int)tempo.TotalSeconds)} segundos";

            if (tempo.TotalHours < 1)
                return $"{(int)tempo.TotalMinutes} min";

            var horas = (int)tempo.TotalHours;
            var minutos = tempo.Minutes;

            return minutos == 0
                ? $"{horas}h"
                : $"{horas}h {minutos}min";
        }
    }

    public string Descricao
    {
        get
        {
            var tipoPessoa = Tipo == TipoPessoaAcesso.Aluno
                ? "Aluno"
                : "Colaborador";

            if (!EhSaida)
                return $"Entrada - {tipoPessoa}";

            return $"Saída - {tipoPessoa} | Permanência: {TempoPermanenciaFormatado}";
        }
    }
}
