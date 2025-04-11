using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Runtime.InteropServices;
using Win_App1.Pages;
using Windows.Graphics;

namespace Win_App1
{
    public sealed partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_SHOWNORMAL = 1;

        private Window? accountWindow; // Зберігає посилання на вікно "Account"

        public MainWindow()
        {
            this.InitializeComponent();

            // Обробити подію закриття основного вікна
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            // Якщо вікно "Account" відкрите, закрити його
            if (accountWindow != null)
            {
                accountWindow.Close();
                accountWindow = null;
            }
        }

        private void MainNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem navigationViewItem)
            {
                string selectedTag = navigationViewItem.Tag?.ToString() ?? string.Empty;

                if (selectedTag == "Account")
                {
                    if (accountWindow == null)
                    {
                        // Create a new window for the "Account" page
                        accountWindow = new Window
                        {
                            Content = new Account()
                        };

                        // Get the window handle
                        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(accountWindow);
                        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                        var appWindow = AppWindow.GetFromWindowId(windowId);

                        // Set the window size
                        appWindow.Resize(new SizeInt32(800, 1000));

                        // Center the window on the screen with a slight vertical offset
                        var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
                        var centerX = (displayArea.WorkArea.Width - 800) / 2;
                        var centerY = (displayArea.WorkArea.Height - 1000) / 2 + 90; // Added offset to move the window lower
                        appWindow.Move(new PointInt32(centerX, centerY));

                        // Optionally set "always on top" (can be removed if not needed)
                        if (appWindow.Presenter is OverlappedPresenter presenter)
                        {
                            presenter.IsAlwaysOnTop = true;
                        }

                        // Activate the window and bring it to the foreground
                        accountWindow.Activate();
                        ShowWindow(hwnd, SW_SHOWNORMAL);
                        SetForegroundWindow(hwnd);

                        // Handle the close event to clear the reference
                        accountWindow.Closed += (s, e) => accountWindow = null;
                    }
                    else
                    {
                        // If the window already exists, bring it to the foreground
                        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(accountWindow);
                        ShowWindow(hwnd, SW_SHOWNORMAL);
                        SetForegroundWindow(hwnd);
                    }
                }
                else
                {
                    // Перейти на інші сторінки в головному вікні
                    string pageName = navigationViewItem.Tag?.ToString() ?? string.Empty;
                    Type pageType = Type.GetType($"Win_App1.Pages.{pageName}")!;
                    ContentFrame.Navigate(pageType, null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
                }

                sender.Header = navigationViewItem.Content?.ToString();
            }
        }
    }
}
