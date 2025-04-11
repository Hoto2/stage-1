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
                        // Створити нове вікно для сторінки "Account"
                        accountWindow = new Window
                        {
                            Content = new Account()
                        };

                        // Отримати дескриптор вікна
                        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(accountWindow);
                        var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hwnd);
                        var appWindow = AppWindow.GetFromWindowId(windowId);

                        // Встановити розмір вікна
                        appWindow.Resize(new SizeInt32(800, 1000));

                        // Примусово встановити "завжди поверх" (можна прибрати, якщо не потрібно)
                        if (appWindow.Presenter is OverlappedPresenter presenter)
                        {
                            presenter.IsAlwaysOnTop = true;
                        }

                        // Активувати вікно та вивести його на передній план
                        accountWindow.Activate();
                        ShowWindow(hwnd, SW_SHOWNORMAL);
                        SetForegroundWindow(hwnd);

                        // Обробити подію закриття, щоб очистити посилання
                        accountWindow.Closed += (s, e) => accountWindow = null;
                    }
                    else
                    {
                        // Якщо вікно вже існує, вивести його на передній план
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
