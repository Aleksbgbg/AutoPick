namespace AutoPick.Persistence.Types
{
    using System.IO;
    using System.Text;

    public class StringType : IReadWriteableType
    {
        private readonly IntType _intType;
        private readonly Encoding _encoding;

        public StringType(IntType intType, Encoding encoding)
        {
            _intType = intType;
            _encoding = encoding;
        }

        public void Write(Stream stream, object value)
        {
            byte[] stringBytes = _encoding.GetBytes((string)value);
            _intType.Write(stream, stringBytes.Length);
            stream.Write(stringBytes, 0, stringBytes.Length);
        }

        public object? Read(Stream stream)
        {
            int length = (int)_intType.Read(stream);

            if (length < 0)
            {
                return null;
            }

            if (length > (stream.Length - stream.Position))
            {
                return null;
            }

            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            return _encoding.GetString(bytes);
        }
    }
}