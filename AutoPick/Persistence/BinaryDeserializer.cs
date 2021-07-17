namespace AutoPick.Persistence
{
    using System.IO;
    using System.Text;

    public class BinaryDeserializer
    {
        private readonly Stream _stream;

        public BinaryDeserializer(Stream stream)
        {
            _stream = stream;
        }

        public ushort DeserializeFieldIndex()
        {
            return ReadUshort();
        }

        public bool DeserializeBool()
        {
            return _stream.ReadByte() == 1;
        }

        public int DeserializeInt()
        {
            return ReadInt();
        }

        public string DeserializeString()
        {
            int length = ReadInt();
            byte[] bytes = new byte[length];
            _stream.Read(bytes, 0, length);
            return Encoding.Unicode.GetString(bytes);
        }

        private ushort ReadUshort()
        {
            int byte0 = _stream.ReadByte();
            int byte1 = _stream.ReadByte();

            return (ushort)((byte1 << 8) | byte0);
        }

        private int ReadInt()
        {
            int byte0 = _stream.ReadByte();
            int byte1 = _stream.ReadByte();
            int byte2 = _stream.ReadByte();
            int byte3 = _stream.ReadByte();

            return (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0;
        }
    }
}