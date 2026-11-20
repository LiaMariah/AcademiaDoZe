using AcademiaDoZe.Application.DTOs.ControleAcesso;
using System;
using System.Collections.Generic;
using System.Text;

namespace AcademiaDoZe.Application.Interfaces
{
    public interface IControleAcessoService
    {
        Task<IReadOnlyList<PessoaAcessoDto>> BuscarPessoasAsync(string termo, TipoPessoaAcesso tipo);

        Task<PessoaAcessoDto?> ObterPessoaAsync(string id, TipoPessoaAcesso tipo);

        Task RegistrarEntradaAsync(string pessoaId, TipoPessoaAcesso tipo);

        Task RegistrarSaidaAsync(string pessoaId, TipoPessoaAcesso tipo);

        Task<IReadOnlyList<RegistroAcessoDto>> ObterAcessosDoDiaAsync(DateTime data);
    }
}
