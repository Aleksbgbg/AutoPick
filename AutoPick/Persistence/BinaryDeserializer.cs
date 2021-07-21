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

        public bool? DeserializeBool()
        {
            int value = _stream.ReadByte();

            if (value == 0)
            {
                return false;
            }

            if (value == 1)
            {
                return true;
            }

            return null;
        }

        public int? DeserializeInt()
        {
            return ReadInt();
        }

        public string? DeserializeString()
        {
            int? lengthNullable = ReadInt();

            if ((lengthNullable == null) || (lengthNullable.Value < 0) || (lengthNullable.Value > RemainingBytes()))
            {
                return null;
            }

            int length = lengthNullable.Value;

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

        private int? ReadInt()
        {
            if (RemainingBytes() < 4)
            {
                return null;
            }

            int byte0 = _stream.ReadByte();
            int byte1 = _stream.ReadByte();
            int byte2 = _stream.ReadByte();
            int byte3 = _stream.ReadByte();

            return (byte3 << 24) | (byte2 << 16) | (byte1 << 8) | byte0;
        }

        private long RemainingBytes()
        {
            return _stream.Length - _stream.Position;
        }
    }
}