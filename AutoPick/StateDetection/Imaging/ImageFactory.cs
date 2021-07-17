namespace AutoPick.StateDetection.Imaging
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using AutoPick.Execution;
    using Emgu.CV;
    using Emgu.CV.CvEnum;

    public static class ImageFactory
    {
        public static WriteableBitmap CreateScreenshotRenderSurface()
        {
            return new(
                AutoPicker.DefaultWindowSize.Width,
                AutoPicker.DefaultWindowSize.Height,
                dpiX: 96,
                dpiY: 96,
                PixelFormats.Bgr32,
                palette: null);
        }

        public static IImage ScreenshotFromPath(string path)
        {
            Bitmap bitmap = (Bitmap)Image.FromFile(path);
            return Utils.UsingBitmapData(bitmap, bitmapData =>
            {
                using Mat imageMat = new(bitmap.Size, DepthType.Cv8U, 3, bitmapData.Scan0, Math.Abs(bitmapData.Stride));

                Mat targetMat = new(AutoPicker.DefaultWindowSize, DepthType.Cv8U, 1);

                if (bitmap.Size == AutoPicker.DefaultWindowSize)
                {
                    CvInvoke.CvtColor(imageMat, targetMat, ColorConversion.Rgb2Gray);
                }
                else
                {
                    using Mat resizeMat = new(AutoPicker.DefaultWindowSize, DepthType.Cv8U, 1);
                    CvInvoke.Resize(imageMat, resizeMat, resizeMat.Size);
                    CvInvoke.CvtColor(resizeMat, targetMat, ColorConversion.Rgb2Gray);
                }

                return new MatImage(targetMat);
            });
        }

        public static unsafe ITemplate TemplateFromStream(Stream stream)
        {
            Bitmap bitmap = (Bitmap)Image.FromStream(stream);
            return Utils.UsingBitmapData(bitmap, bitmapData =>
            {
                Mat dataMat = new(bitmap.Size, DepthType.Cv8U, 1);

                for (int index = 0; index < (bitmap.Width * bitmap.Height); ++index)
                {
                    byte* sourcePixel = (byte*)IntPtr.Add(bitmapData.Scan0, index * 4).ToPointer();
                    byte* targetPixel = (byte*)IntPtr.Add(dataMat.DataPointer, index).ToPointer();

                    *targetPixel = *sourcePixel;
                }

                return new TemplateImage(dataMat);
            });
        }
    }
}