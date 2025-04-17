using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Win_App1.Pages
{
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

            var newTheme = toggleSwitch.IsOn ? ElementTheme.Light : ElementTheme.Dark;
            rootElement.RequestedTheme = newTheme;
        }
    }
}