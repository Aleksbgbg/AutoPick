namespace AutoPick.Util
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    public static class Execute
    {
        private static readonly Dispatcher Dispatcher = Application.Current.Dispatcher;
        
        public static void OnUiThread(Action action)
        {
            Dispatcher.Invoke(action);
        }
    }
}