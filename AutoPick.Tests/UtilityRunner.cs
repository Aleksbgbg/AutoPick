namespace AutoPick.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using AutoPick.Persistence;
    using AutoPick.Runes;
    using AutoPick.WinApi.Native;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.OCR;
    using Emgu.CV.Structure;
    using Xunit;
    using Rune = AutoPick.Runes.Rune;

    // Utilities to help sourcing screenshots and testing features.
    // Ideally these utilities should be made into a utility program.
    // All tests here are disabled to prevent running by default.
    // All tests are parameterised with example parameters - configure for your specific use case.
    // To run a test, enable it and ensure the test case(s) use your required parameters.
    public class UtilityRunner
    {
        private const string SkipReason = "All utilities are skipped by default. Remove skip reason to enable and run.";

        [Theory(Skip = SkipReason)]
        [InlineData(@"A:\Downloads\tessdata-master", @"A:\Text.png", "Hi")]
        public void TestTesseract(string tesseractDataDirectory, string textImagePath, string expectedResult)
        {
            Tesseract tesseract = new(tesseractDataDirectory, "eng", OcrEngineMode.Default);
            tesseract.SetImage(new Image<Rgb, byte>(textImagePath));

            string discoveredText = tesseract.GetOsdText();

            Assert.Equal(expectedResult, discoveredText);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(
            @"A:\Programming\AutoPick\AutoPick\DetectionImages\Pick.png",
            @"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1600x900\BasicScreens\[8]Pick.png",
            @"A:\MatchRegion.png",
            555, 588, 172, 42)]
        public void MatchImageAndSaveMatchRegionToNewImage(string templatePath, string targetImagePath,
                                                           string outputPath,
                                                           int matchX, int matchY, int matchWidth, int matchHeight)
        {
            Image<Gray, byte> template = new(templatePath);
            Image<Gray, byte> matchRegion = new Image<Gray, byte>(targetImagePath)
                                            .Resize(1280, 720, Inter.Lanczos4)
                                            .GetSubRect(new Rectangle(matchX, matchY, matchWidth, matchHeight));

            Image<Gray, float> matchImage = matchRegion.MatchTemplate(template, TemplateMatchingType.CcoeffNormed);
            double minVal = 0;
            double maxVal = 0;
            Point minLoc = Point.Empty;
            Point maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(matchImage, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

            float[,,] matches = matchImage.Data;
            for (int y = 0; y < matches.GetLength(0); y++)
            {
                for (int x = 0; x < matches.GetLength(1); x++)
                {
                    float matchScore = matches[y, x, 0];

                    if (matchScore > 0.5f)
                    {
                        matchRegion.Draw(new Rectangle(x, y, matchWidth - 1, matchHeight - 1), new Gray(1.0));
                    }
                }

            }

            matchRegion.Save(outputPath);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(@"A:\Programming\AutoPick\AutoPick.Tests\TestImages\1280x720\BasicScreens\[9]SelectedHover.png",
                    @"A:\Programming\AutoPick\AutoPick\DetectionImages\SelectedHover.png",
                    555, 588, 172, 42)]
        public void CutImage(string sourceImage, string outputPath, int matchX, int matchY, int width, int height)
        {
            Image<Gray, byte> source = new(sourceImage);
            Image<Gray, byte> target = new(new Size(width, height));

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    target[new Point(x, y)] = source[new Point(x + matchX, y + matchY)];
                }
            }

            target.Save(outputPath);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(0, true, @"A:\Content\League\Capture")]
        public async Task ScreenshotClient(int delayBeforeScreenshotMs, bool infinite, string outputDirectory)
        {
            IntPtr window = Win32Util.FindWindowA(null, "League of Legends");

            IntPtr sourceDeviceContext = Win32Util.GetDC(window);
            IntPtr targetDeviceContext = Win32Util.CreateCompatibleDC(sourceDeviceContext);

            Win32Util.GetWindowRect(window, out Win32Rect windowRect);
            IntPtr bitmap = Win32Util.CreateCompatibleBitmap(sourceDeviceContext, windowRect.Width, windowRect.Height);
            Win32Util.SelectObject(targetDeviceContext, bitmap);

            await Task.Delay(delayBeforeScreenshotMs);

            for (int i = 0; ; ++i)
            {
                Win32Util.PrintWindow(window, targetDeviceContext, PrintWindowParam.PW_CLIENTONLY);
                Image.FromHbitmap(bitmap).ToImage<Rgb, byte>().Save($@"{outputDirectory}\Image{i}.png");

                if (!infinite)
                {
                    break;
                }
            }

            Win32Util.DeleteObject(bitmap);
            Win32Util.DeleteDC(targetDeviceContext);
            Win32Util.ReleaseDC(window, sourceDeviceContext);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(@"B:\Videos\07-04 19-55-35.avi", @"A:\Content\League\Extract")]
        public void ExtractFramesFromVideo(string videoPath, string outputDirectory)
        {
            VideoCapture videoCapture = new(videoPath, VideoCapture.API.Ffmpeg);

            for (int i = 0; ; ++i)
            {
                Mat frame = videoCapture.QueryFrame();

                if (frame == null)
                {
                    break;
                }

                frame.Save($@"{outputDirectory}\{i}.png");
            }
        }

        [Theory(Skip = SkipReason)]
        [InlineData(
            @"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1280x720\BasicScreens\[8]Pick.png",
            @"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1280x720\BasicScreens\[9]SelectedHover.png",
            @"A:\ResizeComparison1.png",
            555, 588, 172, 42)]
        public void CompareRegionBeforeAndAfterResize(string originalImagePath, string imageToResizePath,
                                                      string outputPath,
                                                      int regionX, int regionY, int regionWidth, int regionHeight)
        {
            Image<Rgb, byte> originalImage = new(originalImagePath);
            Image<Rgb, byte> imageToResize = new(imageToResizePath);
            Rectangle targetRegion = new(regionX, regionY, regionWidth, regionHeight);

            Image<Rgb, byte> comparisonOutputImage = new(targetRegion.Width, (targetRegion.Height * 2) + 1);

            Image<Rgb, byte> subregion = originalImage.GetSubRect(targetRegion);
            for (int x = 0; x < targetRegion.Width; ++x)
            {
                for (int y = 0; y < targetRegion.Height; ++y)
                {
                    comparisonOutputImage[y, x] = subregion[y, x];
                }
            }

            subregion = imageToResize.Resize(1280, 720, Inter.Lanczos4).GetSubRect(targetRegion);
            for (int y = targetRegion.Height + 1; y < ((targetRegion.Height * 2) + 1); ++y)
            {
                for (int x = 0; x < (targetRegion.Width); ++x)
                {
                    comparisonOutputImage[y, x] = subregion[y - (targetRegion.Height + 1), x];
                }
            }

            comparisonOutputImage.Draw(new LineSegment2D(new Point(0, targetRegion.Height),
                                                         new Point(targetRegion.Width, targetRegion.Height)),
                                       new Rgb(Color.Red), 1);
            comparisonOutputImage.Save(outputPath);
        }

        [Theory()]
        [InlineData(
            @"A:\Downloads\dragontail-11.15.1\dragontail-11.15.1\11.15.1\data\en_GB\runesReforged.json",
            @"A:\Programming\AutoPick\AutoPick\Data\Runes.bin")]
        public void ParseRunes(string readLocation, string writeLocation)
        {
            string runesDataString = File.ReadAllText(readLocation);
            RunesRead.Type[]? runesData = JsonSerializer.Deserialize<RunesRead.Type[]>(runesDataString);

            RunesFile runesWrite = new();
            List<RuneType> types = new();

            foreach (RunesRead.Type? readType in runesData)
            {
                string typeName = readType.key;

                RuneType writeType = new()
                {
                    Id = readType.id,
                    Name = typeName,
                    Icon = $"/Images/Runes/{typeName}/{typeName}.png"
                };

                List<RuneSlot> slots = new();

                foreach (RunesRead.Slot readSlot in readType.slots)
                {
                    RuneSlot writeSlot = new();
                    List<Rune> runes = new();

                    foreach (RunesRead.Rune readRune in readSlot.runes)
                    {
                        Rune writeRune = new()
                        {
                            Id = readRune.id,
                            Name = readRune.name,
                            Description = ScrubDescription(readRune.shortDesc),
                            Icon = $"/Images/Runes/{typeName}/{Path.GetFileName(readRune.icon)}"
                        };
                        runes.Add(writeRune);
                        // writeSlot.Runes.Add(writeRune);
                    }

                    writeSlot.Runes = runes.ToArray();

                    // writeType.Slots.Add(writeSlot);
                    slots.Add(writeSlot);
                }

                writeType.Slots = slots.ToArray();
                types.Add(writeType);
            }

            runesWrite.Runes = types.ToArray();


            using var f = File.OpenWrite(writeLocation);
            new BinaryReadWriter<RunesFile>().Serialize(runesWrite, f);


            // File.WriteAllText(writeLocation,  JsonSerializer.Serialize(runesWrite));
        }

        private string ScrubDescription(string description)
        {
            StringBuilder builder = new();

            bool isInsideMarkup = false;

            foreach (char c in description)
            {
                if (isInsideMarkup)
                {
                    if (c == '>')
                    {
                        isInsideMarkup = false;
                    }
                }
                else
                {
                    if (c == '<')
                    {
                        isInsideMarkup = true;
                    }
                    else
                    {
                        builder.Append(c);
                    }
                }
            }

            return builder.ToString();
        }

        private class RunesRead
        {
            public class Rune
            {
                public int id { get; set; }

                public string key { get; set; }

                public string icon { get; set; }

                public string name { get; set; }

                public string shortDesc { get; set; }

                public string longDesc { get; set; }
            }

            public class Slot
            {
                public Rune[] runes { get; set; }
            }

            public class Type
            {
                public int id { get; set; }

                public string key { get; set; }

                public string icon { get; set; }

                public string name { get; set; }

                public Slot[] slots { get; set; }
            }
        }

        private class RunesWrite
        {
            public class Rune
            {
                public int Id { get; set; }

                public string Name { get; set; }

                public string Icon { get; set; }

                public string Description { get; set; }
            }

            public class Slot
            {
                public List<Rune> Runes { get; } = new();
            }

            public class Type
            {
                public int Id { get; set; }

                public string Name { get; set; }

                public string Icon { get; set; }

                public List<Slot> Slots { get; } = new();
            }

            public List<Type> Runes { get; } = new();
        }
    }
}