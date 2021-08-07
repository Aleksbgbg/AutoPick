namespace AutoPick.Persistence.Types
{
    using System.IO;
    using System.Text;

    public class StringType : IReadWriteableType
    {
        private readonly IntType _intType;

        public StringType(IntType intType)
        {
            _intType = intType;
        }

        public void Write(Stream stream, object value)
        {
            byte[] stringBytes = Encoding.Unicode.GetBytes((string)value);
            _intType.Write(stream, stringBytes.Length);
            stream.Write(stringBytes, 0, stringBytes.Length);
        }

        public object Read(Stream stream)
        {
            int length = (int)_intType.Read(stream);
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return Encoding.Unicode.GetString(bytes);
        }
    }
}