namespace AutoPick.Persistence.Types.Complex
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class ComplexType : IReadWriteableType
    {
        private readonly bool _isSubMessage;

        private readonly Type _type;

        private readonly IReadWriteableType _ushortReadWriteable;

        private readonly Dictionary<ushort, PropertySerializationInfo> _fieldSerializationInfoByIndex;

        private ComplexType(
            bool isSubMessage,
            Type type,
            IReadWriteableType ushortReadWriteable,
            Dictionary<ushort, PropertySerializationInfo> fieldSerializationInfoByIndex)
        {
            _isSubMessage = isSubMessage;
            _type = type;
            _ushortReadWriteable = ushortReadWriteable;
            _fieldSerializationInfoByIndex = fieldSerializationInfoByIndex;
        }

        public void Write(Stream stream, object value)
        {
            if (_isSubMessage)
            {
                _ushortReadWriteable.Write(stream, CountSerializableFields(value));
            }

            foreach ((ushort fieldIndex, PropertySerializationInfo propertySerializationInfo) in _fieldSerializationInfoByIndex)
            {
                PropertyInfo propertyInfo = propertySerializationInfo.PropertyInfo;
                IReadWriteableType readWriteableType = propertySerializationInfo.ReadWriteable;

                object? memberValue = propertyInfo.GetValue(value);

                if (memberValue == null)
                {
                    continue;
                }

                _ushortReadWriteable.Write(stream, fieldIndex);
                readWriteableType.Write(stream, memberValue);
            }
        }

        private ushort CountSerializableFields(object value)
        {
            return (ushort)(_fieldSerializationInfoByIndex.Values
                                                          .Count(info => info.PropertyInfo.GetValue(value) != null));
        }

        public object? Read(Stream stream)
        {
            object returnValue = Activator.CreateInstance(_type)!;

            HashSet<ushort> deserializedFields = new();

            Func<bool> isFinished;

            if (_isSubMessage)
            {
                ushort fieldsSerialized = (ushort)_ushortReadWriteable.Read(stream);
                isFinished = () => fieldsSerialized == deserializedFields.Count;
            }
            else
            {
                isFinished = () => stream.Position == stream.Length;
            }

            while (!isFinished())
            {
                ushort fieldIndex = (ushort)_ushortReadWriteable.Read(stream);

                if (!_fieldSerializationInfoByIndex.ContainsKey(fieldIndex) || deserializedFields.Contains(fieldIndex))
                {
                    continue;
                }

                PropertySerializationInfo propertySerializationInfo = _fieldSerializationInfoByIndex[fieldIndex];
                PropertyInfo propertyInfo = propertySerializationInfo.PropertyInfo;
                IReadWriteableType readWriteable = propertySerializationInfo.ReadWriteable;

                object propertyValue = readWriteable.Read(stream);
                propertyInfo.SetValue(returnValue, propertyValue);

                deserializedFields.Add(fieldIndex);
            }

            return returnValue;
        }

        public static ComplexType Create(Type type, ReadWriteableTypeStore readWriteableTypeStore)
        {
            return Create(true, type, readWriteableTypeStore);
        }

        public static ComplexType CreateRoot(Type type, ReadWriteableTypeStore readWriteableTypeStore)
        {
            return Create(false, type, readWriteableTypeStore);
        }

        private static ComplexType Create(bool isSubMessage, Type type, ReadWriteableTypeStore readWriteableTypeStore)
        {
            var array = type.GetProperties()
                            .Select(property => new
                            {
                                Property = property,
                                FieldIndexAttribute = property.GetCustomAttribute<FieldIndexAttribute>()
                            })
                            .Where(propertyInfo => propertyInfo.FieldIndexAttribute != null)
                            .ToArray();

            Dictionary<ushort, PropertySerializationInfo> fieldSerializationInfoByIndex = new();

            foreach (var value in array)
            {
                ushort fieldIndex = value.FieldIndexAttribute!.FieldIndex;
                PropertyInfo propertyInfo = value.Property;

                if (fieldSerializationInfoByIndex.ContainsKey(fieldIndex))
                {
                    throw InvalidProperty(propertyInfo, $"has duplicate attribute [FieldIndex({fieldIndex})]");
                }

                if (!IsNullable(propertyInfo))
                {
                    throw InvalidProperty(propertyInfo, "isn't nullable");
                }

                if (!propertyInfo.CanRead)
                {
                    throw InvalidProperty(propertyInfo, "isn't readable");
                }

                if (!propertyInfo.CanWrite)
                {
                    throw InvalidProperty(propertyInfo, "isn't writeable");
                }

                IReadWriteableType readWriteableType = GetReadWriteable(readWriteableTypeStore, propertyInfo);

                fieldSerializationInfoByIndex.Add(
                    fieldIndex, new PropertySerializationInfo(propertyInfo, readWriteableType));
            }

            return new ComplexType(
                isSubMessage,
                type,
                readWriteableTypeStore.SimpleReadWriteable(typeof(ushort?)),
                fieldSerializationInfoByIndex);
        }

        private static IReadWriteableType GetReadWriteable(ReadWriteableTypeStore types, PropertyInfo propertyInfo)
        {
            Type type = propertyInfo.PropertyType;

            if (type.IsArray)
            {
                Type elementType = type.GetElementType();

                if (!types.ContainsTypeForProperty(elementType, propertyInfo))
                {
                    types.AddReadWriteable(elementType, Create(elementType, types));
                }
            }

            if (types.ContainsTypeForProperty(type, propertyInfo))
            {
                return types.PropertyReadWriteable(type, propertyInfo);
            }

            IReadWriteableType readWriteableForType = Create(type, types);
            types.AddReadWriteable(type, readWriteableForType);
            return readWriteableForType;
        }

        private static bool IsNullable(PropertyInfo info)
        {
            if (!info.PropertyType.IsValueType)
            {
                return true;
            }

            return Nullable.GetUnderlyingType(info.PropertyType) != null;
        }

        private static Exception InvalidProperty(PropertyInfo propertyInfo, string problem)
        {
            throw new InvalidOperationException(
                $"Property {propertyInfo.Name} on type {propertyInfo.DeclaringType?.Name} {problem}.");
        }
    }
}