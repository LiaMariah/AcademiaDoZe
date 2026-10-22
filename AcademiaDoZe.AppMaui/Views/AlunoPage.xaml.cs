using AcademiaDoZe.AppMaui.ViewModels;

namespace AcademiaDoZe.AppMaui.Views;

public partial class AlunoPage : ContentPage
{
    public AlunoPage(AlunoViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is AlunoViewModel viewModel)
            await viewModel.InitializeAsync();
    }
}
