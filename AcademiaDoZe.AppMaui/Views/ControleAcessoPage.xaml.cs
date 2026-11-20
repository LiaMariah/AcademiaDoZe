
using AcademiaDoZe.AppMaui.ViewModels;

namespace AcademiaDoZe.AppMaui.Views;

public partial class ControleAcessoPage : ContentPage
{
    private readonly ControleAcessoViewModel _viewModel;

    public ControleAcessoPage(ControleAcessoViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.AtualizarCommand.ExecuteAsync(null);
    }
}

