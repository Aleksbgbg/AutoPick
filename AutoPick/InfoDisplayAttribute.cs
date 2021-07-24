namespace AutoPick
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class InfoDisplayAttribute : Attribute
    {
        public InfoDisplayAttribute(string text, string icon)
        {
            Text = text;
            Icon = icon;
        }

        public string Text { get; }

        public string Icon { get; }
    }
}