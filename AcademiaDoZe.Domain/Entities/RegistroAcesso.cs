
namespace AcademiaDoZe.Domain.Entities;

public class RegistroAcesso
{
    public int Id { get; set; }
    public int PessoaId { get; set; }
    public int TipoPessoa { get; set; }
    public DateTime Entrada { get; set; }
    public DateTime? Saida { get; set; }

    public bool PossuiEntradaAberta => Saida is null;
}
