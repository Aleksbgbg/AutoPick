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

        public object? Read(Stream stream)
        {
            int value = stream.ReadByte();

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
    }
}