using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Win_App1.Pages
{
    public sealed partial class Account : Page
    {
        private bool isRegistrationMode = false;
        public static User CurrentUser { get; private set; }
        private readonly MainWindow _mainWindow;

        public Account(MainWindow mainWindow)
        {
            this.InitializeComponent();
            _mainWindow = mainWindow;
            InitializeUI();
        }

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private void InitializeUI()
        {
            if (this.XamlRoot == null) return;

            // Set theme based on application settings
            if (Application.Current is App app && app.Resources.TryGetValue("RequestedTheme", out var theme))
            {
                this.RequestedTheme = (ElementTheme)theme;
            }

            // Initialize button text
            ActionButton.Content = "Увійти";
            SwitchModeButton.Content = "Реєстрація";
            SetUIMode(false);
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AreControlsInitialized())
            {
                _ = ShowErrorDialog("UI elements not initialized properly");
                return;
            }

            string errorMessage = ValidateInputs();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _ = ShowErrorDialog(errorMessage);
                return;
            }

            _ = ProcessAuthentication();
        }

        private bool AreControlsInitialized()
        {
            return EmailTextBox != null && PasswordBox != null && ConfirmPasswordBox != null;
        }

        private string ValidateInputs()
        {
            var validator = new InputValidator(
                isRegistrationMode ? UsernameTextBox.Text : null,
                EmailTextBox.Text,
                PasswordBox.Password,
                isRegistrationMode ? ConfirmPasswordBox.Password : null,
                isRegistrationMode
            );

            return validator.Validate();
        }

        private async Task ProcessAuthentication()
        {
            CurrentUser = new User(
                isRegistrationMode ? UsernameTextBox.Text : "User",
                EmailTextBox.Text
            );

            string action = isRegistrationMode ? "Реєстрація" : "Вхід";
            string message = isRegistrationMode
                ? $"Користувач {CurrentUser.Username} ({MaskEmail(CurrentUser.Email)}) успішно зареєстрований!"
                : $"Вітаємо! Ви увійшли в систему.";

            await ShowSuccessDialog(action, message);
            NavigateToProfile();

            if (isRegistrationMode)
            {
                SwitchMode();
            }
        }

        private async Task ShowErrorDialog(string message)
        {
            if (this.XamlRoot == null) return;

            await new ContentDialog
            {
                Title = "Помилка",
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }

        private async Task ShowSuccessDialog(string title, string message)
        {
            if (this.XamlRoot == null) return;

            await new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return email;

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0) return email;

            string localPart = email.Substring(0, atIndex);
            int charsToShow = Math.Min(3, localPart.Length);
            string visiblePart = localPart.Substring(0, charsToShow);
            return $"{visiblePart}*****{email.Substring(atIndex)}";
        }

        private void SwitchModeButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchMode();
        }

        private void SwitchMode()
        {
            isRegistrationMode = !isRegistrationMode;
            SetUIMode(isRegistrationMode);
        }

        private void SetUIMode(bool registrationMode)
        {
            ActionButton.Content = registrationMode ? "Зареєструватися" : "Увійти";
            SwitchModeButton.Content = registrationMode ? "Вхід" : "Реєстрація";
            UsernameTextBox.Visibility = registrationMode ? Visibility.Visible : Visibility.Collapsed;
            ConfirmPasswordBox.Visibility = registrationMode ? Visibility.Visible : Visibility.Collapsed;

            // Clear fields when switching modes
            UsernameTextBox.Text = string.Empty;
            ConfirmPasswordBox.Password = string.Empty;

            // Adjust margins for layout
            EmailTextBox.Margin = registrationMode ? new Thickness(0, 190, 0, 0) : new Thickness(0, 120, 0, 0);
            PasswordBox.Margin = registrationMode ? new Thickness(0, 260, 0, 0) : new Thickness(0, 190, 0, 0);

            if (registrationMode)
            {
                ConfirmPasswordBox.Margin = new Thickness(0, 330, 0, 0);
                ActionButton.Margin = new Thickness(0, 400, 0, 0);
                SwitchModeButton.Margin = new Thickness(0, 470, 0, 0);
            }
            else
            {
                ActionButton.Margin = new Thickness(0, 260, 0, 0);
                SwitchModeButton.Margin = new Thickness(0, 330, 0, 0);
            }

            // Update button styling
            SwitchModeButton.Background = registrationMode
                ? (Brush)Resources["SystemControlHighlightAltListAccentLowBrush"]
                : new SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }

        private void NavigateToProfile()
        {
            _mainWindow.NavigateToProfile();

            // Close the Account window
            var app = Application.Current as App;
            if (app?.AccountWindow != null)
            {
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(app.AccountWindow);
                ShowWindow(hwnd, 0); // SW_HIDE
            }
        }
    }

    public class User
    {
        public string Username { get; }
        public string Email { get; }

        public User(string username, string email)
        {
            Username = username;
            Email = email;
        }
    }

    public class InputValidator
    {
        private readonly string username;
        private readonly string email;
        private readonly string password;
        private readonly string confirmPassword;
        private readonly bool isRegistrationMode;

        public InputValidator(string username, string email, string password, string confirmPassword, bool isRegistrationMode)
        {
            this.username = username;
            this.email = email;
            this.password = password;
            this.confirmPassword = confirmPassword;
            this.isRegistrationMode = isRegistrationMode;
        }

        public string Validate()
        {
            string errorMessage = string.Empty;

            if (isRegistrationMode)
            {
                errorMessage += ValidateUsername();
            }

            errorMessage += ValidateEmail();
            errorMessage += ValidatePassword();

            if (isRegistrationMode)
            {
                errorMessage += ValidateConfirmPassword();
            }

            return errorMessage.TrimEnd();
        }

        private string ValidateUsername()
        {
            string error = string.Empty;
            if (string.IsNullOrWhiteSpace(username))
                error += "Ім'я користувача не може бути порожнім.\n";
            else if (username.Length <= 3)
                error += "Ім'я користувача має містити більше 3 символів.\n";
            return error;
        }

        private string ValidateEmail()
        {
            string error = string.Empty;
            if (string.IsNullOrWhiteSpace(email))
                error += "Електронна пошта не може бути порожньою.\n";
            else if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                error += "Введіть коректну електронну пошту.\n";
            return error;
        }

        private string ValidatePassword()
        {
            string error = string.Empty;
            if (string.IsNullOrWhiteSpace(password))
                error += "Пароль не може бути порожнім.\n";
            else if (password.Length <= 7)
                error += "Пароль має містити більше 7 символів.\n";
            return error;
        }

        private string ValidateConfirmPassword()
        {
            string error = string.Empty;
            if (string.IsNullOrWhiteSpace(confirmPassword))
                error += "Підтвердження пароля не може бути порожнім.\n";
            else if (password != confirmPassword)
                error += "Паролі не співпадають.\n";
            return error;
        }
    }
}