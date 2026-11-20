
using AcademiaDoZe.Application.DTOs.ControleAcesso;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Services;

public class ControleAcessoService : IControleAcessoService
{
    private readonly IAlunoService _alunoService;
    private readonly IColaboradorService _colaboradorService;
    private readonly IRegistroAcessoRepository _registroRepository;

    public ControleAcessoService(
        IAlunoService alunoService,
        IColaboradorService colaboradorService,
        IRegistroAcessoRepository registroRepository)
    {
        _alunoService = alunoService;
        _colaboradorService = colaboradorService;
        _registroRepository = registroRepository;
    }

    public async Task<IReadOnlyList<PessoaAcessoDto>> BuscarPessoasAsync(
        string termo,
        TipoPessoaAcesso tipo)
    {
        termo = termo?.Trim() ?? string.Empty;

        var resultado = new List<PessoaAcessoDto>();

        if (tipo == TipoPessoaAcesso.Aluno)
        {
            var alunos = await _alunoService.ObterTodosAsync();

            var filtrados = alunos.Where(a =>
                string.IsNullOrWhiteSpace(termo) ||
                a.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                a.Cpf.Contains(termo) ||
                a.Id.ToString() == termo);

            foreach (var aluno in filtrados)
            {
                resultado.Add(await CriarPessoaAsync(
                    aluno.Id,
                    aluno.Nome,
                    aluno.Cpf,
                    TipoPessoaAcesso.Aluno));
            }
        }
        else if (tipo == TipoPessoaAcesso.Colaborador)
        {
            var colaboradores = await _colaboradorService.ObterTodosAsync();

            var filtrados = colaboradores.Where(c =>
                string.IsNullOrWhiteSpace(termo) ||
                c.Nome.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                c.Cpf.Contains(termo) ||
                c.Id.ToString() == termo);

            foreach (var colaborador in filtrados)
            {
                resultado.Add(await CriarPessoaAsync(
                    colaborador.Id,
                    colaborador.Nome,
                    colaborador.Cpf,
                    TipoPessoaAcesso.Colaborador));
            }
        }

        return resultado;
    }

    public async Task<PessoaAcessoDto?> ObterPessoaAsync(
        string id,
        TipoPessoaAcesso tipo)
    {
        if (!int.TryParse(id, out var pessoaId))
            return null;

        if (tipo == TipoPessoaAcesso.Aluno)
        {
            var aluno = await _alunoService.ObterPorIdAsync(pessoaId);

            return aluno is null
                ? null
                : await CriarPessoaAsync(
                    aluno.Id, aluno.Nome, aluno.Cpf, tipo);
        }

        if (tipo == TipoPessoaAcesso.Colaborador)
        {
            var colaborador =
                await _colaboradorService.ObterPorIdAsync(pessoaId);

            return colaborador is null
                ? null
                : await CriarPessoaAsync(
                    colaborador.Id,
                    colaborador.Nome,
                    colaborador.Cpf,
                    tipo);
        }

        return null;
    }

    public async Task RegistrarEntradaAsync(
        string pessoaId,
        TipoPessoaAcesso tipo)
    {
        if (!int.TryParse(pessoaId, out var id))
            throw new ArgumentException("ID da pessoa inválido.");

        var pessoa = await ObterPessoaAsync(pessoaId, tipo);

        if (pessoa is null)
            throw new KeyNotFoundException("Pessoa não encontrada.");

        var entradaAberta =
            await _registroRepository.ObterEntradaAbertaAsync(
                id, (int)tipo);

        if (entradaAberta is not null)
            throw new InvalidOperationException(
                "Esta pessoa já possui uma entrada aberta.");

        await _registroRepository.RegistrarEntradaAsync(
            new RegistroAcesso
            {
                PessoaId = id,
                TipoPessoa = (int)tipo,
                Entrada = DateTime.Now
            });
    }

    public async Task RegistrarSaidaAsync(
        string pessoaId,
        TipoPessoaAcesso tipo)
    {
        if (!int.TryParse(pessoaId, out var id))
            throw new ArgumentException("ID da pessoa inválido.");

        var entradaAberta =
            await _registroRepository.ObterEntradaAbertaAsync(
                id, (int)tipo);

        if (entradaAberta is null)
            throw new InvalidOperationException(
                "Esta pessoa não possui uma entrada aberta.");

        entradaAberta.Saida = DateTime.Now;

        await _registroRepository.RegistrarSaidaAsync(entradaAberta);
    }

    public async Task<IReadOnlyList<RegistroAcessoDto>> ObterAcessosDoDiaAsync(
        DateTime data)
    {
        var registros = await _registroRepository.ObterPorDataAsync(data);

        var alunos = (await _alunoService.ObterTodosAsync())
            .ToDictionary(a => a.Id, a => a.Nome);

        var colaboradores = (await _colaboradorService.ObterTodosAsync())
            .ToDictionary(c => c.Id, c => c.Nome);

        var eventos = new List<RegistroAcessoDto>();

        foreach (var registro in registros)
        {
            var tipo = (TipoPessoaAcesso)registro.TipoPessoa;

            var nome = tipo == TipoPessoaAcesso.Aluno
                ? alunos.GetValueOrDefault(
                    registro.PessoaId, "Aluno não encontrado")
                : colaboradores.GetValueOrDefault(
                    registro.PessoaId, "Colaborador não encontrado");

            if (registro.Entrada.Date == data.Date)
            {
                eventos.Add(new RegistroAcessoDto
                {
                    Id = $"{registro.Id}-E",
                    PessoaId = registro.PessoaId.ToString(),
                    NomePessoa = nome,
                    Tipo = tipo,
                    Entrada = registro.Entrada,
                    Saida = null
                });
            }

            if (registro.Saida is DateTime saida &&
                saida.Date == data.Date)
            {
                eventos.Add(new RegistroAcessoDto
                {
                    Id = $"{registro.Id}-S",
                    PessoaId = registro.PessoaId.ToString(),
                    NomePessoa = nome,
                    Tipo = tipo,
                    Entrada = registro.Entrada,
                    Saida = saida
                });
            }
        }

        return eventos
            .OrderByDescending(e => e.Saida ?? e.Entrada)
            .ToList();
    }

    private async Task<PessoaAcessoDto> CriarPessoaAsync(
        int id,
        string nome,
        string cpf,
        TipoPessoaAcesso tipo)
    {
        var entradaAberta =
            await _registroRepository.ObterEntradaAbertaAsync(
                id, (int)tipo);

        return new PessoaAcessoDto
        {
            Id = id.ToString(),
            Nome = nome,
            Cpf = cpf,
            Tipo = tipo,
            Situacao = entradaAberta is null
                ? "Sem entrada aberta"
                : "Dentro da academia",
            PossuiEntradaAberta = entradaAberta is not null
        };
    }
}
