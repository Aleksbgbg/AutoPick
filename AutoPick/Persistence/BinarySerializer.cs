namespace AutoPick.Persistence
{
    using System.IO;
    using System.Text;

    public class BinarySerializer
    {
        private readonly Stream _stream;

        public BinarySerializer(Stream stream)
        {
            _stream = stream;
        }

        public void SerializeIndex(ushort fieldIndex)
        {
            WriteUshort(fieldIndex);
        }

        public void Serialize(bool value)
        {
            _stream.WriteByte((byte)(value ? 1 : 0));
        }

        public void Serialize(int value)
        {
            WriteInt(value);
        }

        public void Serialize(string value)
        {
            byte[] stringBytes = Encoding.Unicode.GetBytes(value);
            WriteInt(stringBytes.Length);
            _stream.Write(stringBytes, 0, stringBytes.Length);
        }

        private void WriteUshort(ushort value)
        {
            _stream.WriteByte((byte)(value & 255));
            _stream.WriteByte((byte)((value >> 8) & 255));
        }

        private void WriteInt(int value)
        {
            _stream.WriteByte((byte)(value & 255));
            _stream.WriteByte((byte)((value >> 8) & 255));
            _stream.WriteByte((byte)((value >> 16) & 255));
            _stream.WriteByte((byte)((value >> 24) & 255));
        }
    }
}