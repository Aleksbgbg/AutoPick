namespace AutoPick.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class BinaryReadWriter<T> where T : new()
    {
        private readonly Dictionary<ushort, PropertyInfo> _fieldIndexToPropertyInfo = new();

        public BinaryReadWriter()
        {
            var array = typeof(T).GetProperties()
                                 .Select(property => new
                                 {
                                     Property = property,
                                     FieldIndexAttribute = property.GetCustomAttribute<FieldIndexAttribute>()
                                 })
                                 .Where(propertyInfo => propertyInfo.FieldIndexAttribute != null)
                                 .ToArray();

            foreach (var v in array)
            {
                ushort fieldIndex = v.FieldIndexAttribute!.FieldIndex;
                PropertyInfo propertyInfo = v.Property;

                if (_fieldIndexToPropertyInfo.ContainsKey(fieldIndex))
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

                _fieldIndexToPropertyInfo.Add(fieldIndex, propertyInfo);
            }
        }

        public void Serialize(T value, Stream stream)
        {
            BinarySerializer serializer = new(stream);

            foreach ((ushort fieldIndex, PropertyInfo propertyInfo) in _fieldIndexToPropertyInfo)
            {
                object? memberValue = propertyInfo.GetValue(value);

                serializer.SerializeIndex(fieldIndex);

                switch (memberValue)
                {
                case bool b:
                    serializer.Serialize(b);
                    break;
                case int i:
                    serializer.Serialize(i);
                    break;
                case string s:
                    serializer.Serialize(s);
                    break;
                }
            }
        }

        public T Deserialize(Stream stream)
        {
            T value = new();
            BinaryDeserializer deserializer = new(stream);

            while (stream.Position != stream.Length)
            {
                ushort fieldIndex = deserializer.DeserializeFieldIndex();

                if (!_fieldIndexToPropertyInfo.ContainsKey(fieldIndex))
                {
                    continue;
                }

                PropertyInfo propertyInfo = _fieldIndexToPropertyInfo[fieldIndex];
                Type type = propertyInfo.PropertyType;

                if (type == typeof(bool?))
                {
                    propertyInfo.SetValue(value, deserializer.DeserializeBool());
                }
                else if (type == typeof(int?))
                {
                    propertyInfo.SetValue(value, deserializer.DeserializeInt());
                }
                else if (type == typeof(string))
                {
                    propertyInfo.SetValue(value, deserializer.DeserializeString());
                }
            }

            return value;
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