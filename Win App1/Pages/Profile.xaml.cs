using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Win_App1.Pages
{
    public sealed partial class Profile : Page
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public Profile()
        {
            this.InitializeComponent();
            this.Loaded += Profile_Loaded;
        }

        private void Profile_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserData();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            LoadUserData();
        }

        private void LoadUserData()
        {
            if (Account.CurrentUser != null)
            {
                Username = Account.CurrentUser.Username;
                Email = Account.CurrentUser.Email;

                UsernameText.Text = Username;
                EmailText.Text = Email;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
}