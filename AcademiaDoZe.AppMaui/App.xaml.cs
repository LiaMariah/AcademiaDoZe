using AcademiaDoZe.Presentation.AppMaui;
using Microsoft.Extensions.DependencyInjection;

namespace AcademiaDoZe.AppMaui
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}