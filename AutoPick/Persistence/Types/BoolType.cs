namespace AutoPick.Persistence.Types
{
    using System.IO;

    public class BoolType : IReadWriteableType
    {
        public void Write(Stream stream, object value)
        {
            bool b = (bool)value;
            stream.WriteByte((byte)(b ? 1 : 0));
        }

        public object Read(Stream stream)
        {
            return stream.ReadByte() == 1;
        }
    }
}