namespace AutoPick.Util
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;

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
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None, PropertyChangedCallback)
        );

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
    }
}