namespace AutoPick.StateDetection.Imaging
{
    using System;
    using System.Drawing;
    using System.IO;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public static class ImageFactory
    {
        public static IImage ScreenshotFromBitmapHandle(IntPtr bitmapHandle)
        {
            return new EmguCvImage(Image.FromHbitmap(bitmapHandle).ToImage<Gray, byte>());
        }

        public static IImage ScreenshotFromPath(string path)
        {
            return new EmguCvImage(new Image<Gray, byte>(path));
        }

        public static ITemplate TemplateFromStream(Stream stream)
        {
            return new EmguCvImage(((Bitmap) Image.FromStream(stream)).ToImage<Gray, byte>());
        }
    }
}