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

            foreach (var value in array)
            {
                ushort fieldIndex = value.FieldIndexAttribute!.FieldIndex;
                PropertyInfo propertyInfo = value.Property;

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

                if (memberValue == null)
                {
                    continue;
                }

                serializer.SerializeIndex(fieldIndex);
                Serialize(serializer, propertyInfo, memberValue);
            }
        }

        private static void Serialize(BinarySerializer serializer, PropertyInfo propertyInfo, object? value)
        {
            switch (value)
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
            case Enum:
                Type enumType = Nullable.GetUnderlyingType(propertyInfo.PropertyType).GetEnumUnderlyingType();

                if (enumType == typeof(int))
                {
                    serializer.Serialize((int)value);
                }

                break;
            }
        }

        public T Deserialize(Stream stream)
        {
            T returnValue = new();
            BinaryDeserializer deserializer = new(stream);

            HashSet<ushort> deserializedFields = new();

            while (stream.Position != stream.Length)
            {
                ushort fieldIndex = deserializer.DeserializeFieldIndex();

                if ((!_fieldIndexToPropertyInfo.ContainsKey(fieldIndex)) || (deserializedFields.Contains(fieldIndex)))
                {
                    continue;
                }

                PropertyInfo propertyInfo = _fieldIndexToPropertyInfo[fieldIndex];
                Type type = propertyInfo.PropertyType;

                if (type == typeof(bool?))
                {
                    bool? boolValue = deserializer.DeserializeBool();

                    if (boolValue == null)
                    {
                        break;
                    }

                    propertyInfo.SetValue(returnValue, boolValue);
                }
                else if (type == typeof(int?))
                {
                    int? intValue = deserializer.DeserializeInt();

                    if (intValue == null)
                    {
                        break;
                    }

                    propertyInfo.SetValue(returnValue, intValue);
                }
                else if (type == typeof(string))
                {
                    string? stringValue = deserializer.DeserializeString();

                    if (stringValue == null)
                    {
                        break;
                    }

                    propertyInfo.SetValue(returnValue, stringValue);
                }
                else
                {
                    Type? enumType = Nullable.GetUnderlyingType(type);

                    if (enumType?.IsEnum ?? false)
                    {
                        Type underlyingType = enumType.GetEnumUnderlyingType();

                        if (underlyingType == typeof(int))
                        {
                            int? intValue = deserializer.DeserializeInt();

                            if (intValue == null)
                            {
                                break;
                            }

                            propertyInfo.SetValue(returnValue, Enum.ToObject(enumType, intValue));
                        }
                    }
                }

                deserializedFields.Add(fieldIndex);
            }

            return returnValue;
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