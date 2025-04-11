using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Win_App1.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Account : Page
    {
        public Account()
        {
            this.InitializeComponent();

            // Apply the current theme to the Account page
            if (Application.Current is App app && app.Resources.TryGetValue("RequestedTheme", out var theme))
            {
                this.RequestedTheme = (ElementTheme)theme;
            }
        }
    }
}
