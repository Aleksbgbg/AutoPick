namespace AutoPick.Util
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using AutoPick.ViewModels;
    using Microsoft.Xaml.Behaviors;
    using Microsoft.Xaml.Behaviors.Core;
    using EventTrigger = Microsoft.Xaml.Behaviors.EventTrigger;
    using Interaction = Microsoft.Xaml.Behaviors.Interaction;

    public static class View
    {
        private static readonly Dictionary<Type, Func<object>> ViewFactories = new();

        public static void Register<T>(Func<T> factory)
        {
            ViewFactories.Add(typeof(T), () => factory());
        }

        public static readonly DependencyProperty AttachProperty = DependencyProperty.RegisterAttached(
            "Attach",
            typeof(Type),
            typeof(View),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, AttachChanged)
        );

        public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
            "Model",
            typeof(ViewModelBase),
            typeof(View),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ModelChanged)
        );

        public static readonly DependencyProperty ActionProperty = DependencyProperty.RegisterAttached(
            "Action",
            typeof(Action),
            typeof(View),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, ActionChanged));

        private static void AttachChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ContentControl.ContentProperty, ViewFactories[(Type)e.NewValue]());
        }

        public static void SetAttach(ContentControl element, Type value)
        {
            element.SetValue(AttachProperty, value);
        }

        public static Type GetAttach(ContentControl element)
        {
            return (Type)element.GetValue(AttachProperty);
        }

        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(FrameworkElement.DataContextProperty, e.NewValue);
        }

        public static void SetModel(ContentControl element, ViewModelBase value)
        {
            element.SetValue(AttachProperty, value);
        }

        public static ViewModelBase GetModel(ContentControl element)
        {
            return (ViewModelBase)element.GetValue(AttachProperty);
        }

        private static void ActionChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            EventTrigger eventTrigger = new("MouseDown")
            {
                SourceObject = dependencyObject
            };
            eventTrigger.Actions.Add(new InvokeCommandAction
            {
                Command = new ActionCommand((Action)eventArgs.NewValue)
            });

            Interaction.GetTriggers(dependencyObject).Add(eventTrigger);
        }

        public static void SetAction(FrameworkElement element, Action value)
        {
            element.SetValue(AttachProperty, value);
        }

        public static Action GetAction(FrameworkElement element)
        {
            return (Action)element.GetValue(ActionProperty);
        }
    }
}