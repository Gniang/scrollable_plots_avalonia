using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Live.Avalonia;
using NumSharp;
using ScottPlot.Avalonia;

namespace avalonia_play
{
    public class App : Application
    {
        public static readonly bool IsDebug = true;

        /// <summary>
        /// ウインドウインスタンスの作成
        /// </summary>
        public static Window CreateWindow<T>(string windowName = "") where T : IPageView, new()
        {
            Window? window = null;
            if (IsDebug)
            {
                // デバッグ用。コードを保存したらビューを組み立てなおす。
                var liveWindow = new LiveViewHost(new ViewReloader(), Console.WriteLine) { Name = windowName };
                liveWindow.AttachDevTools();
                liveWindow.StartWatchingSourceFilesForHotReloading();
                window = liveWindow;
            }
            else
            {
                window = new Window() { Name = windowName };
            }
            window.Content = new T();
            ViewReloader.Add<T>(window);
            return window;
        }

        public override void OnFrameworkInitializationCompleted()
        {
            this.Styles.Add(new FluentTheme(new Uri("avares://ControlCatalog/Styles")) { Mode = FluentThemeMode.Light });
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = App.CreateWindow<MainPage>();
                MultiWindow.Init(desktop.MainWindow);
            }
            else
            {
                throw new Exception("Not supported platform.");
            }

            base.OnFrameworkInitializationCompleted();
        }
    }

}