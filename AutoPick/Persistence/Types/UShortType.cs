namespace AutoPick.Persistence.Types
{
    using System.IO;

    public class UShortType : IReadWriteableType
    {
        public void Write(Stream stream, object v)
        {
            ushort value = (ushort)v;
            stream.WriteByte((byte)(value & 255));
            stream.WriteByte((byte)((value >> 8) & 255));
        }

        public object? Read(Stream stream)
        {
            int byte0 = stream.ReadByte();
            int byte1 = stream.ReadByte();

            return (ushort)((byte1 << 8) | byte0);
        }
    }
}