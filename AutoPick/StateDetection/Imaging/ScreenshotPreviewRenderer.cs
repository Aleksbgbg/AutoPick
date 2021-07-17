namespace AutoPick.StateDetection.Imaging
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using AutoPick.WinApi.Native;
    using Emgu.CV;

    public class ScreenshotPreviewRenderer
    {
        private readonly WriteableBitmap _writeableBitmap;

        private readonly int _bitmapBytes;

        private bool _clear = true;

        public ScreenshotPreviewRenderer(WriteableBitmap writeableBitmap)
        {
            _writeableBitmap = writeableBitmap;
            _bitmapBytes = writeableBitmap.BackBufferStride * writeableBitmap.PixelHeight;
        }

        public void Clear()
        {
            if (_clear)
            {
                return;
            }

            _writeableBitmap.Lock();
            Win32Util.RtlZeroMemory(_writeableBitmap.BackBuffer,
                                    _writeableBitmap.BackBufferStride * _writeableBitmap.PixelHeight);
            RefreshRenderRegion();
            _writeableBitmap.Unlock();

            _clear = true;
        }

        public unsafe void UpdatePreview(Mat screenshot)
        {
            _writeableBitmap.Lock();
            Buffer.MemoryCopy(
                screenshot.DataPointer.ToPointer(),
                _writeableBitmap.BackBuffer.ToPointer(),
                _bitmapBytes,
                _bitmapBytes);
            RefreshRenderRegion();
            _writeableBitmap.Unlock();

            _clear = false;
        }

        private void RefreshRenderRegion()
        {
            _writeableBitmap.AddDirtyRect(
                new Int32Rect(0, 0, _writeableBitmap.PixelWidth, _writeableBitmap.PixelHeight));
        }
    }
}