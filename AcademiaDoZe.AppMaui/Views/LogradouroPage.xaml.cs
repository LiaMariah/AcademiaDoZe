using AcademiaDoZe.AppMaui.ViewModels;

namespace AcademiaDoZe.AppMaui.Views;

public partial class LogradouroPage : ContentPage
{
    public LogradouroPage(LogradouroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is LogradouroViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}