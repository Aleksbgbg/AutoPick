namespace AutoPick.Persistence.Types
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

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
            };
        }

        public bool ContainsSimpleType(Type type)
        {
            return _readWriteableTypes.ContainsKey(type);
        }

        public bool ContainsTypeForProperty(Type type, PropertyInfo property)
        {
            return GetReadWriteable(type, property) != null;
        }

        public IReadWriteableType SimpleReadWriteable(Type type)
        {
            return _readWriteableTypes[type];
        }

        public IReadWriteableType PropertyReadWriteable(Type type, PropertyInfo property)
        {
            IReadWriteableType? readWriteableType = GetReadWriteable(type, property);

            if (readWriteableType == null)
            {
                throw new InvalidOperationException($"No IReadWriteableType for {property.PropertyType.Name}.");
            }

            return readWriteableType;
        }

        public void AddReadWriteable(Type type, IReadWriteableType readWriteableType)
        {
            _readWriteableTypes.Add(type, readWriteableType);
        }

        private IReadWriteableType? GetReadWriteable(Type type, PropertyInfo property)
        {
            if (_readWriteableTypes.ContainsKey(type))
            {
                return _readWriteableTypes[type];
            }

            if (type == typeof(string))
            {
                EncodingAttribute? encoding = property.GetCustomAttribute<EncodingAttribute>();
                return new StringType(_intType, encoding?.Encoding ?? EncodingAttribute.DefaultEncoding);
            }

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                return new ArrayType(elementType, _intType, PropertyReadWriteable(elementType, property));
            }

            Type? nullableUnderlyingType = Nullable.GetUnderlyingType(type);

            if (nullableUnderlyingType == null)
            {
                return null;
            }

            if (nullableUnderlyingType.IsEnum)
            {
                Type enumUnderlyingType = nullableUnderlyingType.GetEnumUnderlyingType();
                return new EnumType(nullableUnderlyingType, PropertyReadWriteable(enumUnderlyingType, property));
            }

            return null;
        }
    }
}