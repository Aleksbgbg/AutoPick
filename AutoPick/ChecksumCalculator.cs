namespace AutoPick
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class ChecksumCalculator : IDisposable
    {
        private readonly MemoryStream _memoryStream = new();

        public void Dispose()
        {
            _memoryStream.Dispose();
        }

        public void Add(string championName, byte[] championImage)
        {
            byte[] championNameBytes = Encoding.Unicode.GetBytes(championName);
            _memoryStream.Write(championNameBytes, 0, championNameBytes.Length);
            _memoryStream.Write(championImage, 0, championImage.Length);
        }

        public byte[] Calculate()
        {
            return SHA256.HashData(_memoryStream.ToArray());
        }
    }
}