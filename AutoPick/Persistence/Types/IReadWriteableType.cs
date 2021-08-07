namespace AutoPick.Persistence.Types
{
    using System.IO;

    public interface IReadWriteableType
    {
        void Write(Stream stream, object value);

        object Read(Stream stream);
    }
}