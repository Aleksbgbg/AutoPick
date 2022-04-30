namespace AutoPick.Persistence.Types
{
    using System;
    using System.IO;

    public class ArrayType : IReadWriteableType
    {
        private readonly Type _elementType;

        private readonly IntType _intType;

        private readonly IReadWriteableType _underlyingType;

        public ArrayType(Type elementType, IntType intType, IReadWriteableType underlyingType)
        {
            _elementType = elementType;
            _intType = intType;
            _underlyingType = underlyingType;
        }

        public void Write(Stream stream, object value)
        {
            Array array = (Array)value;
            _intType.Write(stream, array.Length);
            foreach (object subValue in array)
            {
                _underlyingType.Write(stream, subValue);
            }
        }

        public object? Read(Stream stream)
        {
            int length = (int)_intType.Read(stream);
            Array array = Array.CreateInstance(_elementType, length);

            for (int index = 0; index < length; ++index)
            {
                array.SetValue(_underlyingType.Read(stream), index);
            }

            return array;
        }
    }
}