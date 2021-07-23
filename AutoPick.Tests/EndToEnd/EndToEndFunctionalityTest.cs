namespace AutoPick.Tests.EndToEnd
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;
    using AutoPick.WinApi.Native;
    using Xunit;

    public class EndToEndFunctionalityTest : IDisposable
    {
        private readonly TestProcessAndThreadManager _processManager;

        private readonly AutoPickAppController _autoPickAppController;
        private readonly MockAppController _mockAppController;

        public EndToEndFunctionalityTest()
        {
            _processManager = new TestProcessAndThreadManager();
            _autoPickAppController = new AutoPickAppController(_processManager);
            _mockAppController = new MockAppController(_processManager);
        }

        public void Dispose()
        {
            _processManager.Dispose();
        }

        [Theory]
        [InlineData(1024, 576)]
        [InlineData(1280, 720)]
        [InlineData(1600, 900)]
        [InlineData(1920, 1080)]
        [InlineData(2560, 1440)]
        public async Task BasicFlow_DetectsAllStates(int width, int height)
        {
            await _autoPickAppController.Start();

            // Window off
            Assert.Equal(State.NotLaunched, await _autoPickAppController.GetState());

            // Window minimised
            await _mockAppController.Start();
            await _mockAppController.ChangeWindowSize(width, height);
            await _mockAppController.MinimiseWindow();

            Assert.Equal(State.Minimised, await _autoPickAppController.GetState());

            // Default state
            await _mockAppController.RestoreWindow();
            await _mockAppController.EnterHomeScreen();
            Assert.Equal(State.Idle, await _autoPickAppController.GetState());

            // Lobby
            await _mockAppController.EnterLobbyScreen();
            Assert.Equal(State.Lobby, await _autoPickAppController.GetState());

            // Queue
            await _mockAppController.EnterQueue();
            Assert.Equal(State.Queue, await _autoPickAppController.GetState());

            // Declined screen
            await _mockAppController.EnterDeclineScreen();
            Assert.Equal(State.Declined, await _autoPickAppController.GetState());

            // Accept screen
            await _mockAppController.EnterAcceptScreen();
            Assert.Equal(State.Accept, await _autoPickAppController.GetState());
            Assert.True(await _mockAppController.HasAccepted());

            // Accept screen with hover
            await _mockAppController.EnterAcceptScreenAndHoverAcceptButton();
            Assert.Equal(State.Accept, await _autoPickAppController.GetState());
            Assert.True(await _mockAppController.HasAccepted());

            // Accepted screen
            await _mockAppController.EnterMatchAcceptedScreen();
            Assert.Equal(State.Accepted, await _autoPickAppController.GetState());

            // Intermediate screen
            await _mockAppController.EnterIntermediateScreen();
            Assert.Equal(State.ChampSelectTransition, await _autoPickAppController.GetState());

            // Intermediate screen (blank)
            await _mockAppController.EnterIntermediateBlankScreen();
            Assert.Equal(State.ChampSelectTransition, await _autoPickAppController.GetState());

            // Connecting screen
            await _mockAppController.EnterConnectingScreen();
            Assert.Equal(State.Connecting, await _autoPickAppController.GetState());

            // Pick champion
            await _autoPickAppController.SetLane(Lane.Jungle);
            await _autoPickAppController.SetChampion("Fiora");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("jng", await _mockAppController.GetLastChatLine());
            Assert.Equal("Fiora", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());

            // Champ selected (but not locked in) screen
            await _mockAppController.EnterChampSelectedScreen();
            Assert.Equal(State.Selected, await _autoPickAppController.GetState());

            // Champ selected (but not locked in) screen with hover
            await _mockAppController.EnterChampSelectedScreenAndHoverLockInButton();
            Assert.Equal(State.Selected, await _autoPickAppController.GetState());

            // Locked in screen
            await _mockAppController.EnterLockedScreen();
            Assert.Equal(State.Locked, await _autoPickAppController.GetState());

            // In game screen
            await _mockAppController.EnterGame();
            Assert.Equal(State.InGame, await _autoPickAppController.GetState());
        }

        [Fact]
        public async Task PickScreen_CallsLaneMultipleTimes()
        {
            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Mid);
            await _autoPickAppController.SetChampion("Irelia");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Collection(await _mockAppController.GetChatLines(),
                              line => Assert.Equal("mid", line),
                              line => Assert.Equal("mid", line),
                              line => Assert.Equal("mid", line));
        }

        [Fact]
        public async Task SelectedScreen_LocksIn()
        {
            await _mockAppController.Start();
            await _autoPickAppController.Start();

            await _mockAppController.EnterChampSelectedScreen();

            Assert.Equal(State.Selected, await _autoPickAppController.GetState());
            Assert.True(await _mockAppController.HasLockedIn());
        }

        [Fact]
        public async Task ToggleDisableGlobalHotkey_StateIsDisabled()
        {
            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _autoPickAppController.SetLane(Lane.Jungle);
            await _autoPickAppController.SetChampion("Fiora");

            // Disable
            SendDisableSignal();
            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Disabled, await _autoPickAppController.GetState());
            Assert.Equal(string.Empty, await _mockAppController.GetLastChatLine());
            Assert.Equal(string.Empty, await _mockAppController.GetSearchBoxText());
            Assert.False(await _mockAppController.HasSelectedChampion());
            Assert.False(await _mockAppController.HasLockedIn());

            // Enable
            SendEnableSignal();
            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("jng", await _mockAppController.GetLastChatLine());
            Assert.Equal("Fiora", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
        }

        [Fact]
        public async Task WindowChangedBetweenLoops_StillDetected()
        {
            await _mockAppController.Start();
            await _autoPickAppController.Start();

            SendDisableSignal();
            await _mockAppController.Shutdown();
            await _mockAppController.Start();
            SendEnableSignal();
            await _mockAppController.EnterLobbyScreen();

            Assert.Equal(State.Lobby, await _autoPickAppController.GetState());
        }

        private void SendDisableSignal()
        {
            IntPtr window = Win32Util.FindWindowA(null, "AutoPick");
            Assert.NotEqual(0, window.ToInt32());
            IntPtr disableHotkey = new(1);
            Win32Util.SendMessage(window, Message.WM_HOTKEY, disableHotkey, IntPtr.Zero);
        }

        private void SendEnableSignal()
        {
            IntPtr window = Win32Util.FindWindowA(null, "AutoPick");
            Assert.NotEqual(0, window.ToInt32());
            IntPtr enableHotkey = new(2);
            Win32Util.SendMessage(window, Message.WM_HOTKEY, enableHotkey, IntPtr.Zero);
        }

        [Fact]
        public async Task LeagueCovered_AutoPickerNeedsToPreformAction_BringsToFront_TakesAction_Hides()
        {
            _processManager.Start("Cover.exe");
            await _mockAppController.Start();
            await _autoPickAppController.Start();
            Assert.NotEqual(0, Win32Util.FindWindowA(null, "Cover").ToInt32());
            IntPtr leagueWindow = Win32Util.FindWindowA(null, "League of Legends");
            Assert.NotEqual(0, leagueWindow.ToInt32());

            // Accept screen
            await _mockAppController.EnterLobbyScreen();

            await _mockAppController.EnterAcceptScreen();

            Assert.Equal(State.Accept, await _autoPickAppController.GetState());
            Assert.True(await _mockAppController.HasAccepted());
            Assert.False(IsTopmost(leagueWindow));

            // Pick screen
            await _mockAppController.EnterLobbyScreen();
            await _autoPickAppController.SetLane(Lane.Adc);
            await _autoPickAppController.SetChampion("Jayce");

            await _mockAppController.EnterPickScreen();

            Assert.Equal(State.Pick, await _autoPickAppController.GetState());
            Assert.Equal("adc", await _mockAppController.GetLastChatLine());
            Assert.Equal("Jayce", await _mockAppController.GetSearchBoxText());
            Assert.True(await _mockAppController.HasSelectedChampion());
            Assert.True(await _mockAppController.HasLockedIn());
            Assert.False(IsTopmost(leagueWindow));

            // Selected screen
            await _mockAppController.EnterLobbyScreen();

            await _mockAppController.EnterChampSelectedScreen();

            Assert.Equal(State.Selected, await _autoPickAppController.GetState());
            Assert.True(await _mockAppController.HasLockedIn());
            Assert.False(IsTopmost(leagueWindow));
        }

        private static bool IsTopmost(IntPtr window)
        {
            WindowStylesEx windowStyle = Win32Util.GetWindowLongA(window, GetWindowLongParam.GWL_EXSTYLE);
            return (windowStyle & WindowStylesEx.WS_EX_TOPMOST) == WindowStylesEx.WS_EX_TOPMOST;
        }

        [Fact]
        public async Task DetectsInvalidWindowSizes()
        {
            await _mockAppController.Start();
            await _autoPickAppController.Start();
            await _mockAppController.EnterLobbyScreen();

            Assert.Equal(State.Lobby, await _autoPickAppController.GetState());

            // Too small
            await _mockAppController.ChangeWindowSize(1023, 575);

            Assert.Equal(State.InvalidWindowSize, await _autoPickAppController.GetState());

            // Lowest supported size
            await _mockAppController.ChangeWindowSize(1024, 576);

            Assert.Equal(State.Lobby, await _autoPickAppController.GetState());

            // Default size
            await _mockAppController.ChangeWindowSize(1280, 720);

            Assert.Equal(State.Lobby, await _autoPickAppController.GetState());

            // Supported size
            await _mockAppController.ChangeWindowSize(1920, 1080);

            Assert.Equal(State.Lobby, await _autoPickAppController.GetState());
        }

        [Fact]
        public async Task StoresChampionAndLaneOnShutdown()
        {
            await _autoPickAppController.Start();

            await _autoPickAppController.SetChampion("Ahri");
            await _autoPickAppController.SetLane(Lane.Top);
            await _autoPickAppController.Shutdown();

            await _autoPickAppController.Start();
            Assert.Equal("Ahri", await _autoPickAppController.GetChampion());
            Assert.Equal(Lane.Top, await _autoPickAppController.GetLane());

            await _autoPickAppController.SetChampion("Jax");
            await _autoPickAppController.SetLane(Lane.Jungle);
            await _autoPickAppController.Shutdown();
            await _autoPickAppController.Start();

            Assert.Equal("Jax", await _autoPickAppController.GetChampion());
            Assert.Equal(Lane.Jungle, await _autoPickAppController.GetLane());
        }

        [Fact(Timeout = 5_000)]
        public async Task StartTwoInstances_OneAutoCloses()
        {
            Process process1 = _processManager.Start("AutoPick.exe");
            Process process2 = _processManager.Start("AutoPick.exe");
            await Task.WhenAny(process1.WaitForExitAsync(),
                               process2.WaitForExitAsync());

            Assert.Single(Process.GetProcessesByName("AutoPick"));
        }
    }
}