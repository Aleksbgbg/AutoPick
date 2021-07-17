namespace AutoPick.StateDetection.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;

    public static class Utils
    {
        public static unsafe byte PixelAt(IntPtr location, Point offset, int lineWidth)
        {
            IntPtr pixelData = IntPtr.Add(location, (offset.Y * lineWidth) + offset.X);
            byte pixelValue = *(byte*)pixelData.ToPointer();
            return pixelValue;
        }

        public static T UsingBitmapData<T>(Bitmap bitmap, Func<BitmapData, T> action)
        {
            BitmapData bitmapData = LockBitmap(bitmap);

            try
            {
                return action(bitmapData);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
                bitmap.Dispose();
            }
        }

        public static void UsingBitmapData(Bitmap bitmap, Action<BitmapData> action)
        {
            BitmapData bitmapData = LockBitmap(bitmap);

            try
            {
                action(bitmapData);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
                bitmap.Dispose();
            }
        }

        private static BitmapData LockBitmap(Bitmap bitmap)
        {
            return bitmap.LockBits(
                new Rectangle(Point.Empty, bitmap.Size),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);
        }
    }
}