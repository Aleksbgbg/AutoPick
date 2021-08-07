namespace AutoPick.Persistence
{
    using System.IO;
    using AutoPick.Persistence.Types;
    using AutoPick.Persistence.Types.Complex;

    public class BinaryReadWriter<T> where T : new()
    {
        private readonly IReadWriteableType _rootReadWriteable;

        public BinaryReadWriter()
        {
            _rootReadWriteable = ComplexType.CreateRoot(typeof(T), new ReadWriteableTypeStore());
        }

        public void Serialize(T value, Stream stream)
        {
            _rootReadWriteable.Write(stream, value);
        }

        public T Deserialize(Stream stream)
        {
            return (T)_rootReadWriteable.Read(stream);
        }
    }
}