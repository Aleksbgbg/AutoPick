﻿namespace AutoPick.Persistence.Types
{
    using System.IO;

    public class UIntType : IReadWriteableType
    {
        public void Write(Stream stream, object v)
        {
            uint value = (uint)v;
            stream.WriteByte((byte)(value & 255));
            stream.WriteByte((byte)((value >> 8) & 255));
            stream.WriteByte((byte)((value >> 16) & 255));
            stream.WriteByte((byte)((value >> 24) & 255));
        }

        public object Read(Stream stream)
        {
            int byte0 = stream.ReadByte();
            int byte1 = stream.ReadByte();
            int byte2 = stream.ReadByte();
            int byte3 = stream.ReadByte();

            return (uint)((byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0);
        }
    }
}