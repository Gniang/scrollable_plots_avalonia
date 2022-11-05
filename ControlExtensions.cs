using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace avalonia_play;

public static class ControlExtensions
{
    #region bind


    /// <summary>
    /// bind data to avalonia property.
    /// <para>eg.</para>
    /// <para><code language="c#">
    /// new TextBox().SetBind(TextBox.TextProperty, viewModel, nameof(viewModel.Text))
    /// </code></para>
    /// <para>equals</para>
    /// <para><code> 
    /// &lt;Window.DataContext&gt;
    ///     &lt;local:ViewModel /&gt;
    /// &lt;/Window.DataContext&gt;
    /// &lt;TextBox Text="{Binding Text}" /&gt;
    /// </code></para>
    /// </summary>
    /// <param name="control">View control.</param>
    /// <param name="prorperty">Binding property.</param>
    /// <param name="bindingViewModel">Binding view model.</param>
    /// <param name="bindingPath">binding path in view model.</param>
    /// <param name="mode">binding mode.</param>
    /// <returns></returns>
    public static TControl SetBind<TControl, TViewModel>(this TControl control, AvaloniaProperty prorperty, TViewModel bindingViewModel, string bindingPath, BindingMode mode = BindingMode.Default) where TControl : Control
    {
        control.Bind(prorperty, new Binding(bindingPath, mode) { Source = bindingViewModel });
        return control;
    }

    /// <inheritdoc cref="ControlExtensions.SetBind"/>
    public static TControl SetBind<TControl, TViewModel>(this TControl control, AvaloniaProperty prorperty, IBinding bindingItem) where TControl : Control
    {
        control.Bind(prorperty, bindingItem);
        return control;
    }

    #endregion

    #region event

    public static T OnClick<T>(this T button, Action<object?, RoutedEventArgs> action)
    where T : Button
    {
        button.Click += new EventHandler<RoutedEventArgs>(action);
        return button;
    }


    /// <summary>
    /// subscribe event.
    /// <para>
    /// <example>
    /// eg.
    /// <code language="C#">
    /// <para>
    ///   Button.Click += (s, e) => { Debug.WriteLine("Clicked"); };
    /// </para>
    ///      ↓
    /// <para>
    ///   Button.On(nameof(Button.Click), (object? s, RoutedEventArgs e) => { Debug.WriteLine("Clicked"); } );
    /// </para>
    /// </code>
    /// </example>
    /// </para>
    /// </summary>
    public static T On<T, TEvent>(this T control, string eventName, Action<object?, TEvent> eventHandler)
    where T : IControl
    where TEvent : EventArgs
    {
        return On(control, eventName, new EventHandler<TEvent>(eventHandler));
    }


    /// <inheritdoc cref="ControlExtensions.On"/>
    public static T On<T, TEvent>(this T control, string eventName, EventHandler<TEvent> eventHandler)
    where T : IControl
    where TEvent : EventArgs
    {
        var ev = typeof(T).GetEvent(eventName);
        if (ev is null)
        {
            throw new NullReferenceException($"[{typeof(T).FullName}] does not declared event [{eventName}].");
        }

        bool typeCompatible = ev.EventHandlerType?.IsAssignableFrom(typeof(EventHandler<TEvent>)) ?? false;
        if (!typeCompatible)
        {
            throw new MissingMemberException(
                $"[{eventName}] event hander type miss match. Actual type is [{ev.EventHandlerType?.FullName}], Argument type is [{typeof(EventHandler<TEvent>).FullName}]");
        }
        ev.AddEventHandler(control, eventHandler);
        return control;
    }
    #endregion


    #region parent_child

    public static Window? GetOwnerWindow(this IControl control)
    {
        var parent = control;
        while (parent != null)
        {
            if (parent is Window w)
            {
                return w;
            }
            parent = parent.Parent;
        }
        return null;
    }

    /// <summary>
    /// add child controls.
    /// </summary>
    /// <param name="panel">parent control</param>
    /// <param name="controls">child controls</param>
    /// <returns></returns>
    public static T Children<T>(this T panel, IEnumerable<IControl> controls)
    where T : IPanel
    {
        foreach (var c in controls)
        {
            panel.Children.Add(c);
        }
        return panel;
    }

    /// <inheritdoc cref="ControlExtensions.Children"/>
    public static T Children<T>(this T panel, params IControl[] controls)
    where T : IPanel
    {
        return Children(panel, controls.AsEnumerable());
    }

    #endregion



    #region grid


    public static T Columns<T>(this T grid, IEnumerable<DataGridColumn> columns) where T : DataGrid
    {
        foreach (var col in columns)
        {
            grid.Columns.Add(col);
        }
        return grid;
    }
    public static T Columns<T>(this T grid, params DataGridColumn[] columns) where T : DataGrid
    {
        return Columns(grid, columns.AsEnumerable());
    }


    /// <summary>set grid row/column index/span.</summary>
    /// <param name="control">controls that to aligment to grid.</param>
    /// <param name="rowIndex">row index of grid.</param>
    /// <param name="columnIndex">column index of grid.</param>
    /// <param name="rowSpan">row span of grid.</param>
    /// <param name="columnSpan">column span of grid.</param>
    /// <returns></returns>
    public static T SetGrid<T>(this T control,
                                int? rowIndex = null,
                                int? columnIndex = null,
                                int? rowSpan = null,
                                int? columnSpan = null)
    where T : Control
    {
        if (rowIndex != null)
            Grid.SetRow(control, rowIndex.Value);

        if (columnIndex != null)
            Grid.SetColumn(control, columnIndex.Value);

        if (rowSpan != null)
            Grid.SetRowSpan(control, rowSpan.Value);

        if (columnSpan != null)
            Grid.SetColumnSpan(control, columnSpan.Value);

        return control;
    }

    /// <summary>
    /// set column definitions to grid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="grid">grid</param>
    /// <param name="s">column definitions. eg. "1*,30,Auto".</param>
    /// <returns></returns>
    public static T ColumnDefinitions<T>(this T grid, string s)
    where T : Grid
    {
        grid.ColumnDefinitions = new ColumnDefinitions(s);
        return grid;
    }

    /// <summary>
    /// set row definitions to grid.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="grid">grid.</param>
    /// <param name="s">row definitions. eg. "1*,30,Auto".</param>
    /// <returns></returns>
    public static T RowDefinitions<T>(this T grid, string s)
    where T : Grid
    {
        grid.RowDefinitions = new RowDefinitions(s);
        return grid;
    }

    #endregion


    #region dock
    public static T DockLeft<T>(this T control)
    where T : Control
    {
        DockPanel.SetDock(control, Dock.Left);
        return control;
    }

    public static T DockRight<T>(this T control)
        where T : Control
    {
        DockPanel.SetDock(control, Dock.Right);
        return control;
    }

    public static T DockBottom<T>(this T control)
    where T : Control
    {
        DockPanel.SetDock(control, Dock.Bottom);
        return control;
    }
    public static T DockTop<T>(this T control)
    where T : Control
    {
        DockPanel.SetDock(control, Dock.Top);
        return control;
    }
    #endregion

    // //
    // // Summary:
    // //     Gets or sets a parameter to be passed to the Avalonia.Controls.Button.Command.
    // public object? CommandParameter { get; set; }
    // //
    // // Summary:
    // //     Gets or sets an Avalonia.Input.KeyGesture associated with this control
    // public KeyGesture? HotKey { get; set; }
    // //
    // // Summary:
    // //     Gets or sets an System.Windows.Input.ICommand to be invoked when the button is
    // //     clicked.
    // public ICommand? Command { get; set; }
    // public static T Command<T>(this T control, IBinding binding)
    // where T : IControl
    // {
    //     control._set(() => control[!Button.CommandProperty] = binding);

    //     foreach (var c in controls)
    //     {
    //         panel.Children.Add(c);
    //     }
    //     return panel;
    // }


}