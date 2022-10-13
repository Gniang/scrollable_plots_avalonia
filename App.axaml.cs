using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Live.Avalonia;

namespace avalonia_play
{
    public partial class App : Application, ILiveView
    {
        public object CreateView(Window window)
        {
            return new MainWindow();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = new LiveViewHost(this, Console.WriteLine);
                // var window = new Window();
                window.Content = CreateView(window);
                window.StartWatchingSourceFilesForHotReloading();

                desktop.MainWindow = window;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}