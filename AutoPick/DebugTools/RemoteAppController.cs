﻿#if DEBUG
namespace AutoPick.DebugTools
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using AutoPick.Util;
    using AutoPick.ViewModels;
    using AutoPick.Views;

    public class RemoteAppController
    {
        private readonly Application _application;

        private readonly MainViewModel _mainViewModel;
        private readonly MainWindow _mainWindow;

        private readonly DetectionUpdateWaiter _detectionUpdateWaiter;

        private readonly ChampionStore _championStore;

        public RemoteAppController(Application application, MainWindow mainWindow, MainViewModel mainViewModel,
                                   DetectionUpdateWaiter detectionUpdateWaiter, ChampionStore championStore)
        {
            _application = application;
            _mainWindow = mainWindow;
            _mainViewModel = mainViewModel;
            _detectionUpdateWaiter = detectionUpdateWaiter;
            _championStore = championStore;
        }

        public void BeginRemoteControl()
        {
            Task.Factory.StartNew(RemoteControlLoop, TaskCreationOptions.LongRunning);
        }

        private async Task RemoteControlLoop()
        {
            try
            {
                TcpListener tcpListener = new(IPAddress.Any, 5556);
                tcpListener.Start();
                TcpClient client = await tcpListener.AcceptTcpClientAsync();

                Execute.OnUiThread(() =>
                {
                    _mainWindow.Topmost = true;
                    _mainWindow.Top = 10;
                    _mainWindow.Left = 1300;
                });
                _detectionUpdateWaiter.Start();

                await using Stream stream = client.GetStream();
                while (true)
                {
                    int dataSize = stream.ReadByte();

                    if (dataSize == -1)
                    {
                        ErrorReporting.ReportError("Network stream ended unexpectedly");
                    }
                    else if (dataSize == 0)
                    {
                        ErrorReporting.ReportError("Unexpected 0-size message");
                    }
                    else
                    {
                        byte[] data = new byte[dataSize];
                        await stream.ReadAsync(data.AsMemory());
                        byte[] response = await HandleMessage(data);
                        stream.WriteByte((byte)response.Length);
                        await stream.WriteAsync(response.AsMemory());
                    }
                }
            }
            catch (Exception e)
            {
                ErrorReporting.ReportError(e, "RemoteControlLoop exited unexpectedly");
            }
        }

        private Task<byte[]> HandleMessage(byte[] message)
        {
            byte command = message[0];
            Span<byte> commandData = message.AsSpan(1);

            switch (command)
            {
            case 0:
                ErrorReporting.ReportInfo("Ordered to shut down by remote app controller");
                Execute.OnUiThread(() => _application.Shutdown());
                break;
            case 1:
                return GetState();
            case 2:
                _mainViewModel.SelectedLane = (Lane)commandData[0];
                break;
            case 3:
                _mainViewModel.SelectedChampion = _championStore.ChampionByName(Encoding.Unicode.GetString(commandData));
                break;
            case 4:
                return Task.FromResult(new[] { (byte)_mainViewModel.SelectedLane });
            case 5:
                return Task.FromResult(Encoding.Unicode.GetBytes(_mainViewModel.SelectedChampion.Name));
            }

            return Task.FromResult(Array.Empty<byte>());
        }

        private async Task<byte[]> GetState()
        {
            await _detectionUpdateWaiter.Wait();
            return new[] {(byte)_mainViewModel.DetectionInfo.State};
        }
    }
}
#endif