using Microsoft.UI.Xaml;

namespace Win_App1
{
    public partial class App : Application
    {
        public Window AccountWindow { get; set; } // <-- Додай це

        public App()
        {
            this.InitializeComponent();
        }

        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var mainWindow = new MainWindow();
            mainWindow.Activate();
        }
    }
}
