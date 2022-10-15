using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
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
    public partial class MainWindow :
    // Window,
    UserControl
    // ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            var plots = Enumerable.Range(0, 3)
                .Select(i =>
                {
                    var plot = new AvaPlot();
                    var d = (double[,])np.random.rand(Constants.IMG_SIZE.Height, Constants.IMG_SIZE.Width).ToMuliDimArray<double>();
                    var hm = plot.Plot.AddHeatmap(d);
                    return new OffsetHeatmap(plot, new[] { hm });
                })
                .ToList();

            Content = new Grid()
            {
                Background = Brushes.Azure,
            }
                .ColumnDefinitions($"50,*")
                .RowDefinitions($"*")
                .Children(ToArrayFlat<Control>(
                        new TextBox().SetGrid(0, 0, 5),
                        new Grid() { Margin = new Avalonia.Thickness(10, 0, 0, 0) }
                            .ColumnDefinitions("*")
                            .RowDefinitions($"{string.Join(",", "*".Repeat(plots.Count))},Auto")
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
                                    .On(nameof(ScrollBar.Scroll), (object? s, ScrollEventArgs e) => SyncPlotByScroll(s, e))
                                )
                            ).SetGrid(0, 1)
                )
            )
            ;

            void SyncPlotByScroll(object? s, ScrollEventArgs e)
            {
                Debug.WriteLine($"scroll:{e.NewValue}");
                plots.ForEach(x =>
                {
                    x.SetXMin(e.NewValue * 20);
                    x.Refresh();
                });
            }

            // this.WhenActivated(disposables =>
            // {
            //     /* Handle interactions etc. */
            //     plot.RefreshRequest();
            // });

            // DataContext = new MainWindowViewModel();

            // double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            // double[] dataY = new double[] { 1, 4, 9, 16, 25 };
            // AvaPlot avaPlot1 = this.Find<AvaPlot>("AvaPlot1");
            // avaPlot1.Plot.AddScatter(dataX, dataY);
            // // var a = avaPlot1.Plot.GetCoordinate(1, 2);
            // // avaPlot1.Plot.CoordinateFromPixel(new System.Drawing.Point(2, 2));
            // avaPlot1.Refresh();
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