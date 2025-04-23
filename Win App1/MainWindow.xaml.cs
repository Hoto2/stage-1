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

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            (Application.Current as App)?.AccountWindow?.Close();
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

                sender.Header = navigationViewItem.Content?.ToString();
            }
        }

        private void HandleAccountWindow()
        {
            var app = Application.Current as App;
            if (app?.AccountWindow == null)
            {
                app.AccountWindow = new Window
                {
                    Content = new Pages.Account(this),
                    Title = "Account"
                };

                var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(app.AccountWindow);
                var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                var appWindow = AppWindow.GetFromWindowId(windowId);

                // Налаштування розміру та позиції
                appWindow.Resize(new SizeInt32(800, 1000));
                var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                appWindow.Move(new PointInt32(
                    (displayArea.WorkArea.Width - 800) / 2,
                    (displayArea.WorkArea.Height - 1000) / 2 + 90));

                app.AccountWindow.Closed += (s, e) => app.AccountWindow = null;
            }

            var accountHwnd = WinRT.Interop.WindowNative.GetWindowHandle(app.AccountWindow);
            ShowWindow(accountHwnd, SW_SHOWNORMAL);
            SetForegroundWindow(accountHwnd);
        }

        private void CloseAccountWindow()
        {
            var app = Application.Current as App;
            app?.AccountWindow?.Close();
            app.AccountWindow = null;
        }

        private void NavigateToPage(NavigationViewItem navigationViewItem)
        {
            string pageName = navigationViewItem.Tag?.ToString() ?? string.Empty;
            Type pageType = Type.GetType($"Win_App1.Pages.{pageName}")!;
            ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo()
            {
                Effect = SlideNavigationTransitionEffect.FromRight
            });
        }

        public void NavigateToProfile()
        {
            ContentFrame.Navigate(typeof(Pages.Profile), null,
                new SlideNavigationTransitionInfo()
                {
                    Effect = SlideNavigationTransitionEffect.FromRight
                });
        }
    }
}