using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;

namespace Win_App1
{
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private const int SW_SHOWNORMAL = 1;
        private Window? Account; // Змінено з accountWindow на Account

        public MainWindow()
        {
            this.InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            // Закриття вікна "Account", якщо воно відкрите
            Account?.Close();
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem navigationViewItem)
            {
                string selectedTag = navigationViewItem.Tag?.ToString() ?? string.Empty;

                if (selectedTag == "Account")
                {
                    HandleAccountWindow();
                }
                else
                {
                    CloseAccountWindow();
                    NavigateToPage(navigationViewItem);
                }

                // Оновлення заголовка
                sender.Header = navigationViewItem.Content?.ToString();
            }
        }

        private void HandleAccountWindow()
        {
            if (Account == null)
            {
                // Створення нового вікна для сторінки "Account"
                Account = new Window
                {
                    Content = new Pages.Account(),
                    Title = "Account"
                };

                // Синхронізація теми
                var currentTheme = ((FrameworkElement)this.Content).RequestedTheme;
                ((FrameworkElement)Account.Content).RequestedTheme = currentTheme;

                // Налаштування вікна
                ConfigureWindow(Account, 800, 1000, 90);

                Account.Closed += (s, e) => Account = null;
            }
            else
            {
                // Якщо вікно вже існує, підняти його на передній план
                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Account);
                ShowWindow(hwnd, SW_SHOWNORMAL);
                SetForegroundWindow(hwnd);
            }
        }

        private void CloseAccountWindow()
        {
            Account?.Close();
            Account = null;
        }

        private void NavigateToPage(NavigationViewItem navigationViewItem)
        {
            string pageName = navigationViewItem.Tag?.ToString() ?? string.Empty;
            Type pageType = Type.GetType($"Win_App1.Pages.{pageName}")!;
            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void ConfigureWindow(Window window, int width, int height, int verticalOffset)
        {
            // Отримання HWND
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            // Налаштування розміру
            appWindow.Resize(new SizeInt32(width, height));

            // Центрування вікна
            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var centerX = (displayArea.WorkArea.Width - width) / 2;
            var centerY = (displayArea.WorkArea.Height - height) / 2 + verticalOffset;
            appWindow.Move(new PointInt32(centerX, centerY));

            // Встановлення "поверх усіх"
            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsAlwaysOnTop = true;
            }

            // Показати вікно
            ShowWindow(hwnd, SW_SHOWNORMAL);
            SetForegroundWindow(hwnd);
        }
    }
}
