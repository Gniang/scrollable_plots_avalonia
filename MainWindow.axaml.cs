using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
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
using System.Dynamic;

using static EnumerableExtensions;
using System.Collections.Generic;
using Avalonia.Data;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using System.Data;

namespace avalonia_play
{



    public class MainPage : UserControl, IPageView
    {

        public MainPage()
        {
            var plots = Enumerable.Range(0, 1)
                .Select(i =>
                {
                    var plot = new AvaPlot();
                    var testData = (double[,])np.random.rand(Constants.IMG_SIZE.Height, Constants.IMG_SIZE.Width).ToMuliDimArray<double>();
                    var hm = plot.Plot.AddHeatmap(testData, lockScales: false);
                    plot.Configuration.Zoom = false;
                    return new OffsetHeatmap(plot, new[] { hm });
                })
                .ToList();

            var vm = new MainPageViewModel();
            this.DataContext = vm;
            Task.Run(() =>
            {
                var sw = Stopwatch.StartNew();
                var items = new List<Item>();
                var testData = (double[,])np.random.rand(30000, 500).ToMuliDimArray<double>();
                Debug.WriteLine($"dataCreated:{sw.Elapsed}");
                var t = testData.ToDataTable();
                Debug.WriteLine($"toDataTable:{sw.Elapsed}");
                //var t = Array.ForEach(testData, );
                Dispatcher.UIThread.Post(() =>
                {

                    vm.Items = t.AsDataView();
                    Debug.WriteLine($"binded:{sw.Elapsed}");
                });
            });

            Content = new Grid()
            {
                Background = Brushes.AliceBlue,
            }
                .ColumnDefinitions($"120,*")
                .RowDefinitions($"30,Auto,1*,2*")
                // .ColumnDefinitions($"*")
                // .RowDefinitions($"50,*")
                .Children(ToArrayFlat<Control>(
                        new TextBox()
                            .SetGrid(0, 0, 1)
                            ,
                        new Button() { Content = "Second Page", Margin = new Thickness(5, 10, 0, 0) }
                            .SetGrid(1, 0, rowSpan: 1)
                            .On(nameof(Button.Click), new EventHandler<RoutedEventArgs>(BtnClick))
                            ,

                        new Grid() { Margin = new Avalonia.Thickness(10, 0, 0, 0) }
                            .SetGrid(0, 1, rowSpan: 3)
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
                            ,

                        new DataGrid() {AutoGenerateColumns = true }
                            .SetBind(nameof(DataGrid.ItemsProperty), vm, nameof(vm.Items))
                        //   .SetGrid(1, 0)
                            .SetGrid(rowIndex: 3, 1)
                 //.Columns((1..500).Select(x => new DataGridTextColumn() { Binding = new Binding($"Column{x}") }))
                // .Columns(
                //     new DataGridTextColumn() { Header = 1, Binding = new Binding() }
                //     new DataGridTextColumn() { Header = 1 }
                //     new DataGridTextColumn() { Header = 1 }
                // )
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