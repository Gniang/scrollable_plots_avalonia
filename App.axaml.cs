using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using Live.Avalonia;

namespace avalonia_play
{
    public class App : Application, ILiveView
    {

        public object CreateView(Window window)
        {
            return new MainWindow();
        }

        // public override void Initialize()
        // {
        //     AvaloniaXamlLoader.Load(this);
        // }

        public override void OnFrameworkInitializationCompleted()
        {
            this.Styles.Add(new FluentTheme(new Uri("avares://ControlCatalog/Styles")) { Mode = FluentThemeMode.Light });
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