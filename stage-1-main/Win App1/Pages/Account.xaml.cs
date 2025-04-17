using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Text.RegularExpressions;

namespace Win_App1.Pages
{
    public sealed partial class Account : Page
    {
        private bool isRegistrationMode = false;

        public Account()
        {
            this.InitializeComponent();

            if (Application.Current is App app &&
                app.Resources.TryGetValue("RequestedTheme", out var theme))
            {
                this.RequestedTheme = (ElementTheme)theme;
            }

            ActionButton.Content = "Увійти";
            SwitchModeButton.Content = "Реєстрація";
            ConfirmPasswordBox.Visibility = Visibility.Collapsed;

            ActionButton.Margin = new Thickness(0, 330, 0, 0);
            SwitchModeButton.Margin = new Thickness(0, 400, 0, 0);
        }

        private async void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Валідація даних
            string errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage += "Ім'я користувача не може бути порожнім.\n";
            }
            else if (username.Length <= 3)
            {
                errorMessage += "Ім'я користувача має містити більше 3 символів.\n";
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage += "Електронна пошта не може бути порожньою.\n";
            }
            else if (!IsValidEmail(email))
            {
                errorMessage += "Введіть коректну електронну пошту.\n";
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage += "Пароль не може бути порожнім.\n";
            }
            else if (password.Length <= 7)
            {
                errorMessage += "Пароль має містити більше 7 символів.\n";
            }

            if (isRegistrationMode)
            {
                if (string.IsNullOrWhiteSpace(confirmPassword))
                {
                    errorMessage += "Підтвердження пароля не може бути порожнім.\n";
                }
                else if (password != confirmPassword)
                {
                    errorMessage += "Паролі не співпадають.\n";
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Помилка",
                    Content = errorMessage.TrimEnd(),
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };

                await errorDialog.ShowAsync();
                return;
            }

            string action = isRegistrationMode ? "Реєстрація" : "Вхід";
            string maskedEmail = MaskEmail(email);
            string message = isRegistrationMode
                ? $"Користувач {username} ({maskedEmail}) успішно зареєстрований!"
                : $"Вітаємо, {username}! Ви увійшли в систему.";

            ContentDialog successDialog = new ContentDialog
            {
                Title = action,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.Content.XamlRoot
            };

            await successDialog.ShowAsync();

            if (isRegistrationMode)
            {
                SwitchMode();
            }
        }

        private string MaskEmail(string email)
        {
            if (string.IsNullOrEmpty(email)) return email;

            int atIndex = email.IndexOf('@');
            if (atIndex <= 0) return email;

            string localPart = email.Substring(0, atIndex);

            // Відображаємо перші 3 символи або менше, якщо email занадто короткий
            int charsToShow = Math.Min(3, localPart.Length);
            string visiblePart = localPart.Substring(0, charsToShow);

            // Додаємо 5 зірочок
            return $"{visiblePart}*****";
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                return Regex.IsMatch(email, pattern);
            }
            catch
            {
                return false;
            }
        }

        private void SwitchModeButton_Click(object sender, RoutedEventArgs e)
        {
            SwitchMode();
        }

        private void SwitchMode()
        {
            isRegistrationMode = !isRegistrationMode;

            ActionButton.Content = isRegistrationMode ? "Зареєструватися" : "Увійти";
            SwitchModeButton.Content = isRegistrationMode ? "Вхід" : "Реєстрація";

            ConfirmPasswordBox.Visibility = isRegistrationMode ? Visibility.Visible : Visibility.Collapsed;
            ConfirmPasswordBox.Password = string.Empty;

            ActionButton.Margin = isRegistrationMode ? new Thickness(0, 400, 0, 0) : new Thickness(0, 330, 0, 0);
            SwitchModeButton.Margin = isRegistrationMode ? new Thickness(0, 470, 0, 0) : new Thickness(0, 400, 0, 0);

            SwitchModeButton.Background = isRegistrationMode
                ? (Microsoft.UI.Xaml.Media.Brush)Resources["SystemControlHighlightAltListAccentLowBrush"]
                : new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
        }
    }
}