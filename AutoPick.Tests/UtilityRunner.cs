namespace AutoPick.Tests
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.OCR;
    using Emgu.CV.Structure;
    using Xunit;
    using Xunit.Abstractions;

    public class UtilityRunner
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public UtilityRunner(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public enum State
        {
            Default,
            Lobby,
            Queue,
            Accept,
            Decline,
            Connecting,
            Pick,
            Selected,
            Locked
        }

        [DllImport("Kernel32.dll")]
        private static extern int GetLastError();

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [DllImport("User32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);

        [DllImport("User32.dll")]
        private static extern IntPtr GetDC(IntPtr window);

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr deviceContext);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr window, out RECT rect);

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr deviceContext, int x, int y);

        [DllImport("Gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr deviceContext, IntPtr bitmap);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PrintWindow(IntPtr window, IntPtr deviceContext, uint flags);

        [DllImport("User32.dll")]
        private static extern int ReleaseDC(IntPtr window, IntPtr deviceContext);

        [DllImport("Gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteDC(IntPtr deviceContext);

        [DllImport("Gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteObject(IntPtr obj);

        /// <summary>
        ///
        /// </summary>
        /// <param name="image"></param>
        /// <param name="expectedState"></param>
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ClientToScreen(IntPtr window, out POINT point);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        // [DllImport("User32.dll")]
        // private static extern uint SendInput(
        //     uint nInputs,
        //     ref INPUT pInputs,
        //     // [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
        //     int cbSize);

        [DllImport("User32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [Fact]
        public void Tesseract()
        {
            // string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            var _ocr = new Tesseract("A:\\Downloads\\tessdata-master\\", "eng", OcrEngineMode.Default);
            var i = new Image<Rgb, byte>("A:\\Text.png");
            _ocr.SetImage(i);
            string z = _ocr.GetOsdText();
            // var result = .GetCharacters();
            Assert.Equal("hi", _ocr.GetOsdText());
        }

        [Fact]
        public void Match()
        {
            var template = new Image<Gray, byte>("A:\\Connecting.png");
            var p = new Rectangle(32, 644, 72, 10);

            for (int i = 41; i < 67; ++i)
            {
                var s = new Image<Gray, byte>($"A:\\Content\\League\\Capture\\Image{i}.png").GetSubRect(
                    new Rectangle(p.X, p.Y, p.Width, p.Height));

                var r = s.MatchTemplate(template, TemplateMatchingType.CcoeffNormed);
                double minVal = 0;
                double maxVal = 0;
                Point minLoc = Point.Empty;
                Point maxLoc = Point.Empty;
                CvInvoke.MinMaxLoc(r, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

                float[,,] matches = r.Data;
                for (int y = 0; y < matches.GetLength(0); y++)
                {
                    for (int x = 0; x < matches.GetLength(1); x++)
                    {
                        double matchScore = matches[y, x, 0];

                        _testOutputHelper.WriteLine(matchScore.ToString());

                        if (matchScore > 0.5)
                        {
                            s.Draw(new Rectangle(x + 1, y + 1, p.Width, p.Height), new Gray(1), 0);
                        }

                    }

                }

                // _testOutputHelper.WriteLine(PixelsMatch(s, template, p).ToString());
                // _testOutputHelper.WriteLine(string.Join(", ", minVal, maxVal, $"[{minLoc.X}, {minLoc.Y}], [{maxLoc.X}, {maxLoc.Y}]"));


                s.Save($"A:\\Content\\League\\Capture\\result-Image{i}.png");
            }
        }

        [Theory]
        [InlineData("[4]Accept.png", "Accept.png", 646, 383, 107, 20)]
        [InlineData("[5]Accepted.png", "Accepted.png", 619, 383, 119, 20)]
        [InlineData("[5]Decline.png", "Declined.png", 623, 383, 129, 20)]
        public void CutImage(string image, string name, int x, int y, int w, int h)
        {
            int width = w;
            int height = h;

            Image<Gray, byte> target = new(new Size(width, height));

            Image<Gray, byte> source = new($"A:\\Content\\League\\{image}");

            // source.MatchTemplate(target, )

            for (int xs = 0; xs < w; ++xs)
            {
                for (int ys = 0; ys < h; ++ys)
                {
                    target[new Point(xs, ys)] = source[new Point(xs + x, ys + y)];
                }
            }

            target.Save($"A:\\Programming\\AutoPick\\AutoPick\\Detect\\{name}");
        }

        [Fact]
        public void ScreenshotClient()
        {
            IntPtr window = FindWindowA(null, "League of Legends");

            IntPtr sourceDeviceContext = GetDC(window);
            IntPtr targetDeviceContext = CreateCompatibleDC(sourceDeviceContext);

            RECT windowRect = new();
            GetWindowRect(window, out windowRect);

            IntPtr bitmap = CreateCompatibleBitmap
            (
                sourceDeviceContext,
                windowRect.Right - windowRect.Left,
                windowRect.Bottom - windowRect.Top
            );
            SelectObject(targetDeviceContext, bitmap);

            int i = 0;

            Thread.Sleep(3000);

            while (i++ == 0)
            {
                PrintWindow(window, targetDeviceContext, 1 /* PW_CLIENTONLY */);

                Image.FromHbitmap(bitmap).ToImage<Rgb, byte>().Save($"A:\\Content\\League\\Capture\\Image{i++}.png");
            }

            ReleaseDC(window, sourceDeviceContext);
            DeleteDC(targetDeviceContext);
            DeleteObject(bitmap);
        }

        [Fact]
        public void ExtractFramesFromVideo()
        {
            var v = new VideoCapture(@"B:\Videos\07-04 19-55-35.avi", VideoCapture.API.Ffmpeg);

            int i = 0;
            while (++i > 0)
            {
                var f = v.QueryFrame();
                if (f == null)
                {
                    break;
                }

                f.Save($@"A:\Content\League\Extract\{i}.png");
            }
        }

        [Fact]

        public void XD()
        {
            var small =new Image<Rgb, byte>(@"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1280x720\BasicScreens\[7]Connecting.png");
            var large =new Image<Rgb, byte>(@"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1920x1080\ConnectingTransitionFrames\1692.png");
            var target = new Rectangle(28, 640, 80, 20);

            var save = new Image<Rgb, byte>(target.Width, (target.Height * 2) + 1);

           // small.GetSubRect(target).Copy(save, mask);

           var subr = small.GetSubRect(target);

           for (int x = 0; x < target.Width; ++x)
           {
               for (int y = 0; y < (target.Height); ++y)
               {
                   save[y, x] = subr[y, x];
               }
           }

           subr = large.Resize(1280, 720, Inter.Linear).GetSubRect(target);

           for (int y = ((target.Height) + 1); y < ((target.Height * 2) + 1); ++y)
           {
               for (int x = 0; x < (target.Width); ++x)
               {
                   save[y, x] = subr[y - ((target.Height) + 1), x];
               }
           }

           save.Draw(new LineSegment2D(new Point(0, target.Height), new Point(target.Width, target.Height)), new Rgb(Color.Red), 1);



                    // .Resize(1280, 720, Inter.Lanczos4);
                     save.Save("A:\\Resized3.png");
        }

    }
}