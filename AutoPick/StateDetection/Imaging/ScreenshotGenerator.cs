namespace AutoPick.StateDetection.Imaging
{
    using System;
    using System.Drawing;
    using AutoPick.Execution;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Size = System.Drawing.Size;

    public class ScreenshotGenerator : IDisposable
    {
        private readonly Mat _screenMat;
        private readonly Mat _resizedScreenMat;
        private readonly Mat _resizedGrayscaleScreenMat;
        private readonly bool _requiresResize;

        private readonly ScreenshotPreviewRenderer _screenshotPreviewRenderer;

        public ScreenshotGenerator(Size windowSize, ScreenshotPreviewRenderer screenshotPreviewRenderer)
        {
            _screenshotPreviewRenderer = screenshotPreviewRenderer;

            _screenMat = new Mat(windowSize, DepthType.Cv8U, 4);

            if (windowSize == AutoPicker.DefaultWindowSize)
            {
                _resizedScreenMat = _screenMat;
                _requiresResize = false;
            }
            else
            {
                _resizedScreenMat = new Mat(AutoPicker.DefaultWindowSize, DepthType.Cv8U, 4);
                _requiresResize = true;
            }

            _resizedGrayscaleScreenMat = new Mat(AutoPicker.DefaultWindowSize, DepthType.Cv8U, 1);
        }

        ~ScreenshotGenerator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _screenMat.Dispose();
                _resizedScreenMat.Dispose();
                _resizedGrayscaleScreenMat.Dispose();
            }
        }

        public void UpdateWindowSnapshot(Bitmap bitmap)
        {
            CopyBitmapToScreenMat(bitmap);

            if (_requiresResize)
            {
                CvInvoke.Resize(_screenMat, _resizedScreenMat, AutoPicker.DefaultWindowSize);
            }

            CvInvoke.CvtColor(_resizedScreenMat, _resizedGrayscaleScreenMat, ColorConversion.Rgba2Gray);
        }

        private unsafe void CopyBitmapToScreenMat(Bitmap bitmap)
        {
            Utils.UsingBitmapData(bitmap, bitmapData =>
            {
                int absoluteStride = Math.Abs(bitmapData.Stride);

                for (int y = 0; y < bitmapData.Height; ++y)
                {
                    IntPtr source = IntPtr.Add(bitmapData.Scan0, y * bitmapData.Stride);
                    IntPtr target = IntPtr.Add(_screenMat.DataPointer, y * absoluteStride);

                    Buffer.MemoryCopy(source.ToPointer(), target.ToPointer(), absoluteStride, absoluteStride);
                }
            });
        }

        public IImage RetrieveSearchImage()
        {
            return new MatImage(_resizedGrayscaleScreenMat);
        }

        public void UpdateWindowPreview()
        {
            _screenshotPreviewRenderer.UpdatePreview(_resizedScreenMat);
        }
    }
}