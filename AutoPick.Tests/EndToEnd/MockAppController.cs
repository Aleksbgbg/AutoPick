namespace AutoPick.Tests.EndToEnd
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public class MockAppController
    {
        private readonly TestProcessAndThreadManager _processManager;

        private readonly AppCommunicator _appCommunicator = new();

        public MockAppController(TestProcessAndThreadManager processManager)
        {
            _processManager = processManager;
        }

        public async Task Start()
        {
            _processManager.Start("MockApp.exe");
            await _appCommunicator.Connect("localhost", 5555);
        }

        public Task MinimiseWindow()
        {
            return _appCommunicator.Send(20);
        }

        public Task RestoreWindow()
        {
            return _appCommunicator.Send(21);
        }

        public async Task<string> GetLastChatLine()
        {
            string[] chatLines = await GetChatLines();
            return chatLines.Length == 0 ? string.Empty : chatLines[^1];
        }

        public async Task<string[]> GetChatLines()
        {
            return Encoding.Unicode
                           .GetString(await _appCommunicator.Send(22))
                           .Split('\n', StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task<string> GetSearchBoxText()
        {
            return Encoding.Unicode.GetString(await _appCommunicator.Send(23));
        }

        public async Task<bool> HasAccepted()
        {
            return (await _appCommunicator.Send(24))[0] == 1;
        }

        public async Task<bool> HasSelectedChampion()
        {
            return (await _appCommunicator.Send(25))[0] == 1;
        }

        public async Task<bool> HasLockedIn()
        {
            return (await _appCommunicator.Send(26))[0] == 1;
        }

        public Task ChangeWindowSize(int width, int height)
        {
            return _appCommunicator.Send(27, new[]
            {
                (byte)((width) & 255),
                (byte)((width >> 8) & 255),
                (byte)((height) & 255),
                (byte)((height >> 8) & 255)
            });
        }

        public async Task EnterHomeScreen()
        {
            await _appCommunicator.Send(1);
        }

        public async Task EnterLobbyScreen()
        {
            await _appCommunicator.Send(2);
        }

        public async Task EnterQueue()
        {
            await _appCommunicator.Send(3);
        }

        public async Task EnterAcceptScreen()
        {
            await _appCommunicator.Send(4);
        }

        public async Task EnterAcceptScreenAndHoverAcceptButton()
        {
            await _appCommunicator.Send(5);
        }

        public async Task EnterMatchAcceptedScreen()
        {
            await _appCommunicator.Send(6);
        }

        public async Task EnterDeclineScreen()
        {
            await _appCommunicator.Send(7);
        }

        public async Task EnterIntermediateScreen()
        {
            await _appCommunicator.Send(8);
        }

        public async Task EnterIntermediateBlankScreen()
        {
            await _appCommunicator.Send(9);
        }

        public async Task EnterEarlyConnectingScreen()
        {
            await _appCommunicator.Send(10);
        }

        public async Task EnterConnectingScreen()
        {
            await _appCommunicator.Send(11);
        }

        public async Task EnterPickScreen()
        {
            await _appCommunicator.Send(12);
        }

        public async Task EnterChampSelectedScreen()
        {
            await _appCommunicator.Send(13);
        }

        public async Task EnterLockedScreen()
        {
            await _appCommunicator.Send(14);
        }

        public async Task EnterGame()
        {
            await _appCommunicator.Send(15);
        }
    }
}