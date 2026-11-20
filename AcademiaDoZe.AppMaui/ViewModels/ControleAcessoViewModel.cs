
using System.Collections.ObjectModel;
using AcademiaDoZe.Application.DTOs.ControleAcesso;
using AcademiaDoZe.Application.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AcademiaDoZe.AppMaui.ViewModels;

public partial class ControleAcessoViewModel : ObservableObject
{
    private readonly IControleAcessoService _service;

    public ObservableCollection<PessoaAcessoDto> Pessoas { get; } = new();

    public ObservableCollection<RegistroAcessoDto> Historico { get; } = new();

    [ObservableProperty]
    private TipoPessoaAcesso tipoSelecionado = TipoPessoaAcesso.Aluno;

    [ObservableProperty]
    private string termoBusca = string.Empty;

    [ObservableProperty]
    private PessoaAcessoDto? pessoaSelecionada;

    [ObservableProperty]
    private bool carregando;

    [ObservableProperty]
    private string mensagem = string.Empty;

    [ObservableProperty]
    private bool mensagemVisivel;

    [ObservableProperty]
    private bool mensagemSucesso;

    public bool AlunoSelecionado =>
        TipoSelecionado == TipoPessoaAcesso.Aluno;

    public bool ColaboradorSelecionado =>
        TipoSelecionado == TipoPessoaAcesso.Colaborador;

    public bool TemPessoaSelecionada =>
        PessoaSelecionada is not null;

    public bool PodeRegistrar =>
        PessoaSelecionada is not null && !Carregando;

    public string TipoAtual =>
        TipoSelecionado == TipoPessoaAcesso.Aluno
            ? "Aluno"
            : "Colaborador";

    public string TextoBotao =>
        PessoaSelecionada?.PossuiEntradaAberta == true
            ? "Registrar Saída"
            : "Registrar Entrada";

    public ControleAcessoViewModel(IControleAcessoService service)
    {
        _service = service;
    }

    partial void OnTipoSelecionadoChanged(TipoPessoaAcesso value)
    {
        OnPropertyChanged(nameof(AlunoSelecionado));
        OnPropertyChanged(nameof(ColaboradorSelecionado));
        OnPropertyChanged(nameof(TipoAtual));

        PessoaSelecionada = null;
        Pessoas.Clear();

        MensagemVisivel = false;
    }

    partial void OnPessoaSelecionadaChanged(PessoaAcessoDto? value)
    {
        OnPropertyChanged(nameof(TemPessoaSelecionada));
        OnPropertyChanged(nameof(PodeRegistrar));
        OnPropertyChanged(nameof(TextoBotao));
    }

    partial void OnCarregandoChanged(bool value)
    {
        OnPropertyChanged(nameof(PodeRegistrar));
    }

    [RelayCommand]
    private void SelecionarAluno()
    {
        TipoSelecionado = TipoPessoaAcesso.Aluno;
    }

    [RelayCommand]
    private void SelecionarColaborador()
    {
        TipoSelecionado = TipoPessoaAcesso.Colaborador;
    }

    [RelayCommand]
    private async Task BuscarAsync()
    {
        if (Carregando)
            return;

        try
        {
            Carregando = true;
            MensagemVisivel = false;

            PessoaSelecionada = null;
            Pessoas.Clear();

            var resultado = await _service.BuscarPessoasAsync(
                TermoBusca?.Trim() ?? string.Empty,
                TipoSelecionado);

            foreach (var pessoa in resultado)
                Pessoas.Add(pessoa);

            if (Pessoas.Count == 0)
            {
                ExibirMensagem(
                    "Nenhuma pessoa encontrada.",
                    false);
            }
        }
        catch (Exception ex)
        {
            ExibirMensagem(
                $"Erro ao buscar pessoas: {ex.Message}",
                false);
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task RegistrarAcessoAsync()
    {
        if (Carregando || PessoaSelecionada is null)
            return;

        try
        {
            Carregando = true;
            MensagemVisivel = false;

            var pessoa = PessoaSelecionada;

            if (pessoa.PossuiEntradaAberta)
            {
                await _service.RegistrarSaidaAsync(
                    pessoa.Id,
                    TipoSelecionado);

                ExibirMensagem(
                    $"Saída registrada para {pessoa.Nome}.",
                    true);
            }
            else
            {
                await _service.RegistrarEntradaAsync(
                    pessoa.Id,
                    TipoSelecionado);

                ExibirMensagem(
                    $"Entrada registrada para {pessoa.Nome}.",
                    true);
            }

            // Atualiza a situação da pessoa após o registro.
            PessoaSelecionada = await _service.ObterPessoaAsync(
                pessoa.Id,
                TipoSelecionado);

            await CarregarHistoricoAsync();
        }
        catch (Exception ex)
        {
            ExibirMensagem(
                $"Erro ao registrar acesso: {ex.Message}",
                false);
        }
        finally
        {
            Carregando = false;
        }
    }

    [RelayCommand]
    private async Task AtualizarAsync()
    {
        if (Carregando)
            return;

        try
        {
            Carregando = true;

            await CarregarHistoricoAsync();
        }
        catch (Exception ex)
        {
            ExibirMensagem(
                $"Erro ao atualizar histórico: {ex.Message}",
                false);
        }
        finally
        {
            Carregando = false;
        }
    }

    private async Task CarregarHistoricoAsync()
    {
        var registros = await _service.ObterAcessosDoDiaAsync(
            DateTime.Today);

        Historico.Clear();

        foreach (var registro in registros)
            Historico.Add(registro);
    }

    private void ExibirMensagem(string texto, bool sucesso)
    {
        Mensagem = texto;
        MensagemSucesso = sucesso;
        MensagemVisivel = true;
    }
}
