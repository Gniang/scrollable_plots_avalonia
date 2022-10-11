using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Logging;
using Avalonia.ReactiveUI;
using NumSharp;
using ReactiveUI;
using ScottPlot.Avalonia;
using ScottPlot.Plottable;

namespace avalonia_play
{
    public partial class MainWindow :
    Window
    // ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            var plot = new ScottPlot.Avalonia.AvaPlot() { };
            var d = (double[,])np.random.rand(200, 10_000).ToMuliDimArray<double>();
            plot.Plot.AddHeatmap(d);

            Content = new DockPanel()
            {
                LastChildFill = true,
            }
                .Children(new Control[]{
                    new TextBox().DockTop(),
                    new ScrollBar() { Name = "hscrollbar", Orientation = Avalonia.Layout.Orientation.Horizontal }
                        .DockBottom()
                        .On("Scroll", (object? s, ScrollEventArgs e) => SyncPlotByScroll(s, e)),
                    plot,
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