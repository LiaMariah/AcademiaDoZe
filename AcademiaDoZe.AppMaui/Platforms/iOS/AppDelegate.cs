using AcademiaDoZe.Presentation.AppMaui;
using Foundation;

namespace AcademiaDoZe.AppMaui
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
