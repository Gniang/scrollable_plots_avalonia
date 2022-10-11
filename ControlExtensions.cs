using System;
using Avalonia.Controls;

namespace avalonia_play;

public static class ControlExtensions
{
    public static T Children<T>(this T panel, params IControl[] controls)
    where T : IPanel
    {
        foreach (var c in controls)
        {
            panel.Children.Add(c);
        }
        return panel;
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