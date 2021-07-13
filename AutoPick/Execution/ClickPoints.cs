namespace AutoPick.Execution
{
    using System;
    using System.Drawing;
    using AutoPick.Win32.Native;

    // All points are relative to the default resolution
    public class ClickPoints
    {
        private static readonly Win32Point AcceptButtonDefaultLocation = new(638, 557);

        private static readonly Win32Point ChatBoxDefaultLocation = new(43, 685);

        private static readonly Win32Point SearchBoxDefaultLocation = new(761, 103);

        private static readonly Win32Point FirstChampionSelectionImageDefaultLocation = new(384, 161);

        private static readonly Win32Point LockInButtonDefaultLocation = new(640, 609);

        public ClickPoints(Size resolution)
        {
            if ((resolution.Width == AutoPicker.DefaultWindowWidth)
                && (resolution.Height == AutoPicker.DefaultWindowHeight))
            {
                AcceptButton = AcceptButtonDefaultLocation;
                ChatBox = ChatBoxDefaultLocation;
                SearchBox = SearchBoxDefaultLocation;
                FirstChampionSelectionImage = FirstChampionSelectionImageDefaultLocation;
                LockInButton = LockInButtonDefaultLocation;
            }
            else
            {
                AcceptButton = Scale(AcceptButtonDefaultLocation, resolution);
                ChatBox = Scale(ChatBoxDefaultLocation, resolution);
                SearchBox = Scale(SearchBoxDefaultLocation, resolution);
                FirstChampionSelectionImage = Scale(FirstChampionSelectionImageDefaultLocation, resolution);
                LockInButton = Scale(LockInButtonDefaultLocation, resolution);
            }
        }

        public Win32Point AcceptButton { get; }

        public Win32Point ChatBox { get; }

        public Win32Point SearchBox { get; }

        public Win32Point FirstChampionSelectionImage { get; }

        public Win32Point LockInButton { get; }

        private static Win32Point Scale(Win32Point point, Size resolution)
        {
            return new(ScaleX(point.X, resolution.Width), ScaleY(point.Y, resolution.Height));
        }

        private static int ScaleX(double value, double newResolution)
        {
            return Scale(value, newResolution, AutoPicker.DefaultWindowWidth);
        }

        private static int ScaleY(double value, double newResolution)
        {
            return Scale(value, newResolution, AutoPicker.DefaultWindowHeight);
        }

        private static int Scale(double value, double newScale, double originalScale)
        {
            return (int)Math.Round((value / originalScale) * newScale);
        }
    }
}