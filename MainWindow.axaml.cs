using System;
using System.ComponentModel;
using System.Diagnostics;
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

namespace avalonia_play
{
    public partial class MainWindow :
    // Window,
    UserControl
    // ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            var plot = new ScottPlot.Avalonia.AvaPlot() { };
            var d = (double[,])np.random.rand(200, 10_0).ToMuliDimArray<double>();
            plot.Plot.AddHeatmap(d);

            Content = new Grid()
            {
                Background = Brushes.AliceBlue,
            }
                .RowDefinitions("50,*,Auto")
                .Children(new Control[]{
                    new TextBox().SetGrid(0),
                    plot.SetGrid(1),
                    new ScrollBar() { Orientation = Avalonia.Layout.Orientation.Horizontal }
                        .SetGrid(2)
                        .On(nameof(ScrollBar.Scroll), (object? s, ScrollEventArgs e) => SyncPlotByScroll(s, e)),
                })
            ;

            static void SyncPlotByScroll(object? s, ScrollEventArgs e)
            {
                Debug.WriteLine($"scroll:{e.NewValue}");
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