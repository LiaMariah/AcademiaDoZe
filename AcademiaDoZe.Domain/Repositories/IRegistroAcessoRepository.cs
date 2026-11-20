
using AcademiaDoZe.Domain.Entities;

namespace AcademiaDoZe.Domain.Repositories;

public interface IRegistroAcessoRepository
{
    Task<RegistroAcesso?> ObterEntradaAbertaAsync(
        int pessoaId,
        int tipoPessoa);

    Task<IReadOnlyList<RegistroAcesso>> ObterPorDataAsync(
        DateTime data);

    Task RegistrarEntradaAsync(RegistroAcesso registro);

    Task RegistrarSaidaAsync(RegistroAcesso registro);
}
