using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;

namespace avalonia_play;

public static class ControlExtensions
{

    public static T OnClick<T>(this T button, Action<object?, RoutedEventArgs> action)
    where T : Button
    {
        button.Click += new EventHandler<RoutedEventArgs>(action);
        return button;
    }

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

    public static TControl Bind<TControl, TBindingItem>(this TControl control, TBindingItem value) where TControl : Control
    {
        // control.Bind(AvaloniaProperty.RegisterDirect(), new Binding());
        return control;
    }

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

    public static T Children<T>(this T panel, IEnumerable<IControl> controls)
    where T : IPanel
    {
        foreach (var c in controls)
        {
            panel.Children.Add(c);
        }
        return panel;
    }
    public static T Children<T>(this T panel, params IControl[] controls)
    where T : IPanel
    {
        return Children(panel, controls.AsEnumerable());
    }

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

    public static T ColumnDefinitions<T>(this T grid, string s)
    where T : Grid
    {
        grid.ColumnDefinitions = new ColumnDefinitions(s);
        return grid;
    }

    public static T RowDefinitions<T>(this T grid, string s)
    where T : Grid
    {
        grid.RowDefinitions = new RowDefinitions(s);
        return grid;
    }

    /// <summary>
    /// Subscribe Event
    /// <para>
    /// <example>
    /// eg.
    /// <code language="C#">
    /// <para>
    ///   Button.Click += (s, e) => { Debug.WriteLine("Clicked"); };
    /// </para>
    ///      ↓
    /// <para>
    ///   Button.On("Click", (object? s, RoutedEventArgs e) => { Debug.WriteLine("Clicked"); } );
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

    /// <summary>
    /// Subscribe Event
    /// <para>
    /// <example>
    /// eg.
    /// <code language="C#">
    /// <para>
    ///   Button.Click += (s, e) => { Debug.WriteLine("Clicked"); };
    /// </para>
    ///      ↓
    /// <para>
    ///   Button.On("Click", (object? s, RoutedEventArgs e) => { Debug.WriteLine("Clicked"); } );
    /// </para>
    /// </code>
    /// </example>
    /// </para>
    /// </summary>
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