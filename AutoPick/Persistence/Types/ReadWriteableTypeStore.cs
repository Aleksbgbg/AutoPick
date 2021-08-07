namespace AutoPick.Persistence.Types
{
    using System;
    using System.Collections.Generic;

    public class ReadWriteableTypeStore
    {
        private readonly Dictionary<Type, IReadWriteableType> _readWriteableTypes;
        private readonly IntType _intType;

        public ReadWriteableTypeStore()
        {
            UShortType uShortType = new();
            _intType = new IntType();
            UIntType uIntType = new();
            _readWriteableTypes = new Dictionary<Type, IReadWriteableType>
            {
                [typeof(bool?)] = new BoolType(),
                [typeof(ushort)] = uShortType,
                [typeof(ushort?)] = uShortType,
                [typeof(int)] = _intType,
                [typeof(int?)] = _intType,
                [typeof(uint)] = uIntType,
                [typeof(uint?)] = uIntType,
                [typeof(string)] = new StringType(_intType),
            };
        }

        public bool ContainsTypeFor(Type type)
        {
            return GetReadWriteable(type) != null;
        }

        public IReadWriteableType ReadWriteableFor(Type type)
        {
            IReadWriteableType? readWriteableType = GetReadWriteable(type);

            if (readWriteableType == null)
            {
                throw new InvalidOperationException($"No IReadWriteableType for {type.Name}.");
            }

            return readWriteableType;
        }

        public void AddReadWriteable(Type type, IReadWriteableType readWriteableType)
        {
            _readWriteableTypes.Add(type, readWriteableType);
        }

        private IReadWriteableType? GetReadWriteable(Type type)
        {
            if (_readWriteableTypes.ContainsKey(type))
            {
                return _readWriteableTypes[type];
            }

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                return new ArrayType(elementType, _intType, ReadWriteableFor(elementType));
            }

            Type? nullableUnderlyingType = Nullable.GetUnderlyingType(type);

            if (nullableUnderlyingType == null)
            {
                return null;
            }

            if (nullableUnderlyingType.IsEnum)
            {
                Type enumUnderlyingType = nullableUnderlyingType.GetEnumUnderlyingType();
                return new EnumType(nullableUnderlyingType, ReadWriteableFor(enumUnderlyingType));
            }

            return null;
        }
    }
}