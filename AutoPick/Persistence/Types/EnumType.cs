namespace AutoPick.Persistence.Types
{
    using System;
    using System.IO;

    public class EnumType : IReadWriteableType
    {
        private readonly Type _enumType;

        private readonly IReadWriteableType _underlyingTypeReadWriteable;

        public EnumType(Type enumType, IReadWriteableType underlyingTypeReadWriteable)
        {
            _enumType = enumType;
            _underlyingTypeReadWriteable = underlyingTypeReadWriteable;
        }

        public void Write(Stream stream, object value)
        {
            _underlyingTypeReadWriteable.Write(stream, value);
        }

        public object Read(Stream stream)
        {
            object read = _underlyingTypeReadWriteable.Read(stream);
            return Enum.ToObject(_enumType, read);
        }
    }
}