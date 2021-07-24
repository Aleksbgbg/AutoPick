namespace AutoPick.Tests.EndToEnd
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;
    using AutoPick.WinApi;
    using AutoPick.WinApi.Native;
    using Xunit;

    // These tests can be flaky when ran all together, as they interfere with the program shutdown / startup process
    public class StabilityTest : IDisposable
    {
        private readonly TestProcessAndThreadManager _processManager;

        private readonly AutoPickAppController _autoPickAppController;
        private readonly MockAppController _mockAppController;

        public StabilityTest()
        {
            _processManager = new TestProcessAndThreadManager();
            _autoPickAppController = new AutoPickAppController(_processManager);
            _mockAppController = new MockAppController(_processManager);
        }

        public void Dispose()
        {
            _processManager.Dispose();
        }

        [Fact]
        public async Task ChampionImagesMissing_ReDownloads()
        {
            if (Directory.Exists("Champions"))
            {
                Directory.Delete("Champions", recursive: true);
            }

            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Adc);
            await _autoPickAppController.SetChampion("Akshan");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("adc", await _mockAppController.GetLastChatLine());
            Assert.Equal("Akshan", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
        }

        [Fact]
        public async Task CorruptingChampionImage_ReDownloads()
        {
            if (!Directory.Exists("Champions"))
            {
                await _autoPickAppController.Start();
                await _autoPickAppController.Shutdown();
            }

            await File.WriteAllBytesAsync("Champions/Akshan.png", new byte[] { 1, 2, 3 });

            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Adc);
            await _autoPickAppController.SetChampion("Akshan");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("adc", await _mockAppController.GetLastChatLine());
            Assert.Equal("Akshan", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
        }

        [Fact]
        public async Task DeletingChampionImage_ReDownloads()
        {
            if (!Directory.Exists("Champions"))
            {
                await _autoPickAppController.Start();
                await _autoPickAppController.Shutdown();
            }

            File.Delete("Champions/Akshan.png");

            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Adc);
            await _autoPickAppController.SetChampion("Akshan");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("adc", await _mockAppController.GetLastChatLine());
            Assert.Equal("Akshan", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
        }

        [Fact]
        public async Task AddExtraChampion_NotSelectable()
        {
            if (!Directory.Exists("Champions"))
            {
                await _autoPickAppController.Start();
                await _autoPickAppController.Shutdown();
            }

            File.Copy("Champions/Akshan.png", "Champions/John.png", overwrite: true);

            await _autoPickAppController.Start();
            await Assert.ThrowsAsync<InvalidOperationException>(() => _autoPickAppController.SetChampion("John"));
        }

        // Requires manual condition setup, so this test is disabled by default
        // Turn off internet connection or shut down server manually
        [Fact(Skip = "Requires manual setup - read test comments.")]
        public async Task InternetOffline_SelfExtractsChampionImages()
        {
            if (Directory.Exists("Champions"))
            {
                Directory.Delete("Champions", recursive: true);
            }
            // TODO: Add WinApi calls here to disable the internet connection or alternatively docker server setup

            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Adc);
            await _autoPickAppController.SetChampion("Akshan");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("adc", await _mockAppController.GetLastChatLine());
            Assert.Equal("Akshan", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
        }

        [Fact]
        public async Task SaveFileLaneCorrupted_NegativeValue_PicksMidLaneByDefault()
        {
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Jungle);
            await _autoPickAppController.Shutdown();
            File.WriteAllBytes("AutoPickData.bin", new byte[]
            {
                // Lane field
                2, 0,
                // Value out of range
                255, 255, 255, 255
            });

            await _autoPickAppController.Start();

            Assert.Equal(Lane.Mid, await _autoPickAppController.GetLane());
        }

        [Fact]
        public async Task SaveFileLaneCorrupted_PositiveValue_PicksMidLaneByDefault()
        {
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Jungle);
            await _autoPickAppController.Shutdown();
            File.WriteAllBytes("AutoPickData.bin", new byte[]
            {
                // Lane field
                2, 0,
                // Value out of range
                5, 0, 0, 0
            });

            await _autoPickAppController.Start();

            Assert.Equal(Lane.Mid, await _autoPickAppController.GetLane());
        }

        [Fact]
        public async Task SaveFileChampionNameCorrupted_PicksDefaultChampion()
        {
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Jungle);
            await _autoPickAppController.Shutdown();
            File.WriteAllBytes("AutoPickData.bin", new byte[]
            {
                // Champion name field
                0, 0,
                // Value out of range
                10, 0, 0, 0, // Length (10 bytes)
                72, 0, 101, 0, 108, 0, 108, 0, 111, 0 // 'Hello'
            });

            await _autoPickAppController.Start();

            Assert.Equal("Katarina", await _autoPickAppController.GetChampion());
        }

        [Fact]
        public async Task SaveFileDeleted_LaunchesDefaultSettings()
        {
            File.Delete("AutoPickData.bin");

            await _autoPickAppController.Start();

            Assert.Equal(Lane.Mid, await _autoPickAppController.GetLane());
            Assert.Equal("Katarina", await _autoPickAppController.GetChampion());
        }

        // Flaky with lower delays
        // System is not 100% stable when fast user input events are applied, however it is sufficiently resilient
        [Fact]
        public async Task ConstantMouseMovement_DoesNotImpairActions()
        {
            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Adc);
            await _autoPickAppController.SetChampion("Jayce");
            _processManager.StartThreadWithTimeout(10_000, async token =>
            {
                InputQueue inputQueue = new(IntPtr.Zero);

                while (!token.IsCancellationRequested)
                {
                    inputQueue.MoveMouse(new Win32Point(5, 5));
                    inputQueue.Flush();
                    await Task.Delay(20);
                    inputQueue.MoveMouse(new Win32Point(500, 5));
                    inputQueue.Flush();
                    await Task.Delay(20);
                }
            });

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("adc", await _mockAppController.GetLastChatLine());
            Assert.Equal("Jayce", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
        }
    }
}