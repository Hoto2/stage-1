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
            if (sender is ToggleSwitch toggleSwitch)
            {
                // Set the theme on the root element
                if (this.XamlRoot.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = toggleSwitch.IsOn
                        ? ElementTheme.Dark
                        : ElementTheme.Light;
                }
            }
        }
    }
}