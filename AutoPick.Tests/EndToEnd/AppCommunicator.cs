namespace AutoPick.Tests.EndToEnd
{
    using System;
    using System.IO;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class AppCommunicator
    {
        private readonly TcpClient _tcpClient = new();

        private Stream _stream;

        public async Task Connect(string address, int port)
        {
            try
            {
                await _tcpClient.ConnectAsync(address, port);
            }
            catch (SocketException e)
            {
                throw new InvalidOperationException("Target app cannot be connected to - it likely crashed", e);
            }

            _stream = _tcpClient.GetStream();
            _stream.ReadTimeout = 10_000;
        }

        public Task<byte[]> Send(byte command)
        {
            return Send(new[] { command });
        }

        public Task<byte[]> Send(byte command, byte[] data)
        {
            byte[] dataToSend = new byte[data.Length + 1];
            dataToSend[0] = command;
            Array.Copy(data, 0, dataToSend, 1, data.Length);

            return Send(dataToSend);
        }

        private async Task<byte[]> Send(byte[] data)
        {
            _stream.WriteByte((byte)data.Length);
            await _stream.WriteAsync(data);

            int responseSize = _stream.ReadByte();

            if (responseSize == -1)
            {
                throw new InvalidOperationException("Remote app has no response - it may have crashed");
            }

            if (responseSize == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] response = new byte[responseSize];
            await _stream.ReadAsync(response.AsMemory());
            return response;
        }
    }
}