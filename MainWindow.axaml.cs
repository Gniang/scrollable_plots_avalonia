using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using Live.Avalonia;
using NumSharp;
using ReactiveUI;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;

using static EnumerableExtensions;

namespace avalonia_play
{



    public class MainPage : UserControl, IPageView
    {

        public MainPage()
        {
            var plots = Enumerable.Range(0, 3)
                .Select(i =>
                {
                    var plot = new AvaPlot();
                    var testData = (double[,])np.random.rand(Constants.IMG_SIZE.Height, Constants.IMG_SIZE.Width).ToMuliDimArray<double>();
                    var hm = plot.Plot.AddHeatmap(testData, lockScales: false);
                    plot.Configuration.Zoom = false;
                    return new OffsetHeatmap(plot, new[] { hm });
                })
                .ToList();

            Content = new Grid()
            {
                Background = Brushes.Azure,
            }
                .ColumnDefinitions($"120,*")
                .RowDefinitions($"30,Auto,*")
                .Children(ToArrayFlat<Control>(
                        new TextBox().SetGrid(0, 0, 1),
                        new Button() { Content = "Second Page", Margin = new Thickness(5, 10, 0, 0) }
                            .SetGrid(1, 0, rowSpan: 1)
                            .On(nameof(Button.Click), new EventHandler<RoutedEventArgs>(BtnClick)),

                        new Grid() { Margin = new Avalonia.Thickness(10, 0, 0, 0) }
                            .ColumnDefinitions("*")
                            .RowDefinitions($"{"*".Repeat(plots.Count).JoinString(",")},Auto")
                            .Children(ToArrayFlat<Control>(
               plots.Select((plot, i) =>
                                {
                                    plot.SetXMin(0).SetXZoom(500);
                                    plot.View.SetGrid(i);
                                    return plot.View;
                                }),
                                new ScrollBar()
                                {
                                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                                }
                                    .SetGrid(plots.Count + 1)
                                    .On(nameof(ScrollBar.Scroll), new EventHandler<ScrollEventArgs>(SyncPlotByScroll))
                                )
                            )
                            .SetGrid(0, 1, rowSpan: 3)
                )
            )
            ;

            void BtnClick(object? sender, RoutedEventArgs e)
            {
                this.Navigate(App.CreateWindow<SecondWindow>());
            }

            void SyncPlotByScroll(object? s, ScrollEventArgs e)
            {
                Debug.WriteLine($"scroll:{e.NewValue}");
                plots.ForEach(x =>
                {
                    x.SetXMin(e.NewValue / 100 * Constants.IMG_SIZE.Width);
                    x.Refresh();
                });
            }
        }
    }
    public class SecondWindow : UserControl, IPageView
    {
        public SecondWindow()
        {
            this.Content = new StackPanel()
                .Children(
                    new Button() { Content = "main activate" }
                        .OnClick((s, e) =>
                        {
                            var mainPage = this.FindWindow<MainPage>();
                            mainPage?.GetOwnerWindow()?.Activate();
                        }),
                    new Button() { Content = "close" }
                        .OnClick((s, e) =>
                        {
                            this.GetOwnerWindow()?.Close();
                        })
                );

        }
    }

    public class MainWindowViewModel :
        // ReactiveObject, IActivatableViewModel,
        INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        string buttonText = "Click!!";

        public MainWindowViewModel() : base()
        {
            // this.WhenActivated((disposables) =>
            // {
            //     /* handle activation */
            //     Disposable
            //         .Create(() => { /* handle deactivation */ })
            //         .DisposeWith(disposables);
            // });
        }

        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ButtonText)));
            }
        }

        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        public void ButtonClicked() => ButtonText = "Clicked!!!";
    }
}