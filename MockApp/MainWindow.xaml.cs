﻿namespace MockApp
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;

    public partial class MainWindow
    {
        private const string DebugLogFile = "MockApp.debug.log";
        private const string InfoLogFile = "MockApp.info.log";

        private readonly ReadableAfterRelease<bool> _accepted = new();
        private readonly ReadableAfterRelease<bool> _selectedChamp = new();
        private readonly ReadableAfterRelease<bool> _lockedIn = new();
        private readonly ReadableAfterRelease<StringBuilder> _chatLog = new();

        public MainWindow()
        {
            InitializeComponent();

            File.Delete(DebugLogFile);
            File.Delete(InfoLogFile);

            Task.Factory.StartNew(RemoteControlLoop, TaskCreationOptions.LongRunning);

            LoadScreen(1);
        }

        private async Task RemoteControlLoop()
        {
            try
            {
                TcpListener tcpListener = new(IPAddress.Any, 5555);
                tcpListener.Start();
                TcpClient client = await tcpListener.AcceptTcpClientAsync();

                await using Stream stream = client.GetStream();
                while (true)
                {
                    int dataSize = stream.ReadByte();

                    if (dataSize == -1)
                    {
                        ReportError("Network stream empty");
                    }
                    else if (dataSize == 0)
                    {
                        ReportError("0-sized message");
                    }
                    else
                    {
                        byte[] data = new byte[dataSize];
                        await stream.ReadAsync(data.AsMemory());

                        byte[] response = await HandleMessage(data);

                        ReportInfo($"Responding with {response.Length}-length byte array ({string.Join(", ", response)} | {Encoding.Unicode.GetString(response)})");
                        stream.WriteByte((byte)response.Length);
                        await stream.WriteAsync(response.AsMemory());
                    }
                }
            }
            catch (Exception e)
            {
                ReportError(e, "RemoteControlLoop terminated unexpectedly");
            }
        }

        private async Task<byte[]> HandleMessage(byte[] data)
        {
            byte command = data[0];

            if ((0 < command) && (command < 20))
            {
                await OnUiThread(() => LoadScreen(command));
            }
            else switch (command)
            {
            case 20:
                await OnUiThread(() => WindowState = WindowState.Minimized);
                break;
            case 21:
                await OnUiThread(() => WindowState = WindowState.Normal);
                break;
            case 22:
                StringBuilder chatLog = await _chatLog.Value();
                return Encoding.Unicode.GetBytes(chatLog.ToString());
            case 23:
                return await OnUiThread(() => Encoding.Unicode.GetBytes(SearchBox.Text));
            case 24:
                return new[] { (byte)(await _accepted.Value() ? 1 : 0) };
            case 25:
                return new[] { (byte)(await _selectedChamp.Value() ? 1 : 0) };
            case 26:
                return new[] { (byte)(await _lockedIn.Value() ? 1 : 0) };
            case 27:
                await OnUiThread(() =>
                {
                    Width = (data[2] << 8) | (data[1]);
                    Height = (data[4] << 8) | (data[3]);
                });
                break;
            }

            return Array.Empty<byte>();
        }

        private void LoadScreen(int screenId)
        {
            FileStream fileStream = File.OpenRead($"Screens\\Screen ({screenId}).png");

            BitmapImage image = new();
            image.BeginInit();
            image.StreamSource = fileStream;
            image.EndInit();

            Screen.Source = image;

            ResetScreenState(screenId);
        }

        private void ResetScreenState(int screenId)
        {
            _accepted.Write(false);
            _selectedChamp.Write(false);
            _lockedIn.Write(false);
            _chatLog.Write(log => log.Clear());

            ChatBox.Text = string.Empty;
            SearchBox.Text = string.Empty;

            ChatBox.Visibility = Visibility.Hidden;
            SearchBox.Visibility = Visibility.Hidden;
            AcceptButton.Visibility = Visibility.Hidden;
            SelectChampButton.Visibility = Visibility.Hidden;
            LockInButton.Visibility = Visibility.Hidden;

            if (screenId == 12)
            {
                ChatBox.Visibility = Visibility.Visible;
                SearchBox.Visibility = Visibility.Visible;
                SelectChampButton.Visibility = Visibility.Visible;
            }
            else if (screenId == 13)
            {
                LockInButton.Visibility = Visibility.Visible;
            }
            else if ((screenId == 14))
            {
                ChatBox.Visibility = Visibility.Visible;
            }
            else if ((screenId == 4) || (screenId == 5))
            {
                AcceptButton.Visibility = Visibility.Visible;
            }
        }

        private void Accept(object sender, RoutedEventArgs e)
        {
            _accepted.WriteAndRelease(true);
        }

        private void SelectChamp(object sender, RoutedEventArgs e)
        {
            _selectedChamp.WriteAndRelease(true);
            LockInButton.Visibility = Visibility.Visible;
        }

        private void LockIn(object sender, RoutedEventArgs e)
        {
            _lockedIn.WriteAndRelease(true);
        }

        private void ChatBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _chatLog.WriteAndRelease(log =>
                {
                    log.Append(ChatBox.Text);
                    log.Append('\n');
                });
                ChatBox.Text = "";
            }
        }

        private static async Task OnUiThread(Action action)
        {
            await Application.Current.Dispatcher.InvokeAsync(action);
        }

        private static async Task<T> OnUiThread<T>(Func<T> action)
        {
            T value = default;
            await Application.Current.Dispatcher.InvokeAsync(() => value = action());
            return value;
        }

        private static void ReportInfo(string message, [CallerFilePath] string caller = null, [CallerLineNumber] int lineNumber = 0)
        {
            LogReport(InfoLogFile, message, caller, lineNumber);
        }

        private static void ReportError(Exception exception, string message, [CallerFilePath] string caller = null, [CallerLineNumber] int lineNumber = 0)
        {
            ReportError(string.Join("\n", exception.Message, exception.StackTrace, message), caller, lineNumber);

            if (exception.InnerException != null)
            {
                ReportError(exception.InnerException, message, caller, lineNumber);
            }
        }

        private static void ReportError(string message, [CallerFilePath] string caller = null, [CallerLineNumber] int lineNumber = 0)
        {
            LogReport(DebugLogFile, message, caller, lineNumber);
        }

        private static void LogReport(string file, string message, string caller, int lineNumber)
        {
            using StreamWriter streamWriter = File.AppendText(file);
            streamWriter.WriteLine($"[{Path.GetFileName(caller)}:{lineNumber} @ {DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}] {message}");
        }
    }
}