using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Win_App1.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        public Settings()
        {
            this.InitializeComponent();
        }

        private void ThemeToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleSwitch toggleSwitch || this.XamlRoot.Content is not FrameworkElement rootElement)
                return;

            // Reverse the theme logic: Light theme when toggle is ON, Dark theme when toggle is OFF
            rootElement.RequestedTheme = toggleSwitch.IsOn ? ElementTheme.Light : ElementTheme.Dark;
        }
    }
}