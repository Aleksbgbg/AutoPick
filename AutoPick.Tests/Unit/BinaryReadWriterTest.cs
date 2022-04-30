namespace AutoPick.Tests.Unit
{
    using System;
    using System.IO;
    using System.Text;
    using AutoPick.Persistence;
    using Xunit;

    public class BinaryReadWriterTest
    {
        [Fact]
        public void SerializesBool()
        {
            BinaryReadWriter<BoolData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new BoolData { Value = true }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                255, 255,
                // Value
                1
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesBool()
        {
            BinaryReadWriter<BoolData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new BoolData { Value = true }, memoryStream);
            memoryStream.Position = 0;

            BoolData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(true, data.Value);
        }

        [Fact]
        public void SerializesInt()
        {
            BinaryReadWriter<IntData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new IntData { Value = int.MaxValue }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                255, 255,
                // Value
                255, 255, 255, 127
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesInt()
        {
            BinaryReadWriter<IntData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new IntData { Value = int.MaxValue / 2 }, memoryStream);
            memoryStream.Position = 0;

            IntData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(int.MaxValue / 2, data.Value);
        }

        [Fact]
        public void SerializesEnum()
        {
            BinaryReadWriter<EnumData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new EnumData { Value = Enum.B }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                0, 0,
                // Value
                1, 0, 0, 0
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesEnum()
        {
            BinaryReadWriter<EnumData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new EnumData { Value = Enum.C }, memoryStream);
            memoryStream.Position = 0;

            EnumData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(Enum.C, data.Value);
        }

        [Fact]
        public void SerializesUtf8String()
        {
            BinaryReadWriter<Utf8StringData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new Utf8StringData { Value = "abcdef" }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                255, 255,
                // Byte length
                6, 0, 0, 0,
                // UTF-8 encoded string
                97, 98, 99, 100, 101, 102
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesUtf8String()
        {
            BinaryReadWriter<Utf8StringData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new Utf8StringData { Value = "hello" }, memoryStream);
            memoryStream.Position = 0;

            Utf8StringData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal("hello", data.Value);
        }

        [Fact]
        public void SerializesUtf16String()
        {
            BinaryReadWriter<Utf16StringData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new Utf16StringData { Value = "abcdef" }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                255, 255,
                // Byte length
                12, 0, 0, 0,
                // UTF-16 encoded string
                97, 0, 98, 0, 99, 0, 100, 0, 101, 0, 102, 0
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesUtf16String()
        {
            BinaryReadWriter<Utf16StringData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new Utf16StringData { Value = "hello" }, memoryStream);
            memoryStream.Position = 0;

            Utf16StringData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal("hello", data.Value);
        }

        [Fact]
        public void SerializesArray()
        {
            BinaryReadWriter<ArrayData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new ArrayData
            {
                Array = new MixedData[]
                {
                    new()
                    {
                        Bool = true,
                        Int = 12,
                        String = "Hello!",
                        Enum = Enum.B
                    },
                    new()
                    {
                        Bool = false,
                        Int = null,
                        String = "!olleH",
                        Enum = Enum.C
                    },
                }
            }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                0, 0,
                // Array entries
                2, 0, 0, 0,
                // Array entry 0 - serialized fields
                4, 0,
                // Field index
                0, 0,
                // Bool
                1,
                // Field index
                1, 0,
                // Int
                12, 0, 0, 0,
                // Field index
                2, 0,
                // String length
                6, 0, 0, 0,
                // String
                72, 101, 108, 108, 111, 33,
                // Field index
                3, 0,
                // Enum
                1, 0, 0, 0,
                // Array entry 1 - serialized fields
                3, 0,
                // Field index
                0, 0,
                // Bool
                0,
                // Field index
                2, 0,
                // String length
                6, 0, 0, 0,
                // String
                33, 111, 108, 108, 101, 72,
                // Field index
                3, 0,
                // Enum
                2, 0, 0, 0,
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesArray()
        {
            BinaryReadWriter<ArrayData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new ArrayData
            {
                Array = new MixedData[]
                {
                    new()
                    {
                        Bool = true,
                        Int = 12,
                        String = "Hello!",
                        Enum = Enum.B
                    },
                    new()
                    {
                        Bool = false,
                        Int = null,
                        String = "!olleH",
                        Enum = Enum.C
                    },
                }
            }, memoryStream);
            memoryStream.Position = 0;

            ArrayData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(true, data.Array[0].Bool);
            Assert.Equal(12, data.Array[0].Int);
            Assert.Equal("Hello!", data.Array[0].String);
            Assert.Equal(Enum.B, data.Array[0].Enum);
            Assert.Equal(false, data.Array[1].Bool);
            Assert.Equal(null, data.Array[1].Int);
            Assert.Equal("!olleH", data.Array[1].String);
            Assert.Equal(Enum.C, data.Array[1].Enum);
        }

        [Fact]
        public void SerializesMultipleData()
        {
            BinaryReadWriter<MultipleData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new MultipleData
            {
                Value0 = 5,
                Value1 = 10,
                Value2 = 15
            }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index
                0, 0,
                // Value0
                5, 0, 0, 0,
                // Field index
                1, 0,
                // Value1
                10, 0, 0, 0,
                // Field index
                2, 0,
                // Value2
                15, 0, 0, 0
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesMultipleData()
        {
            BinaryReadWriter<MultipleData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new MultipleData
            {
                Value0 = 5,
                Value1 = 10,
                Value2 = 15
            }, memoryStream);
            memoryStream.Position = 0;

            MultipleData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(5, data.Value0);
            Assert.Equal(10, data.Value1);
            Assert.Equal(15, data.Value2);
        }

        [Fact]
        public void SerializesNestedData()
        {
            BinaryReadWriter<NestedData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new NestedData
            {
                MixedData = new MixedData
                {
                    Bool = true,
                    Int = 456,
                    String = "Hello!",
                    Enum = Enum.B
                },
                MultipleData = new MultipleData
                {
                    Value0 = 125,
                    Value1 = null,
                    Value2 = -73
                }
            }, memoryStream);

            Assert.Equal(new byte[]
            {
                // Field index (nested field)
                0, 0,
                // Number of fields serialized
                4, 0,
                // Field index
                0, 0,
                // Bool
                1,
                // Field index
                1, 0,
                // Int
                200, 1, 0, 0,
                // Field index
                2, 0,
                // String length
                6, 0, 0, 0,
                // String
                72, 101, 108, 108, 111, 33,
                // Field index
                3, 0,
                // Enum
                1, 0, 0, 0,
                // Field index (nested field)
                2, 0,
                // Number of fields serialized
                2, 0,
                // Field index
                0, 0,
                // Value0
                125, 0, 0, 0,
                // Field index
                2, 0,
                // Value2
                183, 255, 255, 255
            }, memoryStream.ToArray());
        }

        [Fact]
        public void DeserializesNestedData()
        {
            BinaryReadWriter<NestedData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new NestedData
            {
                MixedData = new MixedData
                {
                    Bool = true,
                    Enum = Enum.B,
                    Int = 456,
                    String = "Hello!"
                },
                MultipleData = new MultipleData
                {
                    Value0 = 125,
                    Value1 = null,
                    Value2 = -73
                }
            }, memoryStream);
            memoryStream.Position = 0;

            NestedData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(true, data.MixedData.Bool);
            Assert.Equal(Enum.B, data.MixedData.Enum);
            Assert.Equal(456, data.MixedData.Int);
            Assert.Equal("Hello!", data.MixedData.String);
            Assert.Equal(125, data.MultipleData.Value0);
            Assert.Equal(null, data.MultipleData.Value1);
            Assert.Equal(-73, data.MultipleData.Value2);
            Assert.Equal(null, data.NullField);
        }

        [Fact]
        public void HandlesMultipleSerializations()
        {
            BinaryReadWriter<MixedData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new MixedData
            {
                Bool = true,
                Int = 250,
                String = "Hello",
                Enum = Enum.C
            }, memoryStream);
            memoryStream.Position = 0;
            MixedData data1 = binaryReadWriter.Deserialize(memoryStream);
            memoryStream.Position = 0;
            binaryReadWriter.Serialize(data1, memoryStream);
            memoryStream.Position = 0;
            MixedData data2 = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(true, data2.Bool);
            Assert.Equal(250, data2.Int);
            Assert.Equal("Hello", data2.String);
            Assert.Equal(Enum.C, data2.Enum);
        }

        [Fact]
        public void HandlesMissingValues()
        {
            BinaryReadWriter<MixedData> binaryReadWriter = new();
            MemoryStream memoryStream = new(); // No data serialized

            MixedData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Null(data.Bool);
            Assert.Null(data.Int);
            Assert.Null(data.String);
        }

        [Fact]
        public void HandlesUnannotatedValues()
        {
            BinaryReadWriter<PartiallyUnannotatedData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new PartiallyUnannotatedData
            {
                Bool = true,
                Int = 2350749,
                String = "hello"
            }, memoryStream);
            memoryStream.Position = 0;

            PartiallyUnannotatedData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(true, data.Bool);
            Assert.Null(data.Int);
            Assert.Equal("hello", data.String);
        }

        [Theory]
        [InlineData(new byte[] // Bool
        {
            // Field index
            0, 0,
            // Invalid bool (not 0 or 1)
            2
        })]
        [InlineData(new byte[] // String
        {
            // Field index
            2, 0,
            // Invalid string (length bigger than actual readable bytes)
            255, 255, 231, 0, 149, 10, 5, 218, 8, 51, 7, 176, 205, 65, 93, 34, 252, 244, 99, 150
        })]
        [InlineData(new byte[] // String
        {
            // Field index
            2, 0,
            // Invalid string (length negative)
            0, 0, 0, 255, 149, 10, 5, 218, 8, 51, 7, 176, 205, 65, 93, 34, 252, 244, 99, 150
        })]
        public void HandlesIntentionallyInvalidData(byte[] bytes)
        {
            BinaryReadWriter<MixedData> binaryReadWriter = new();
            MemoryStream memoryStream = new(bytes);

            MixedData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Null(data.Bool);
            Assert.Null(data.Int);
            Assert.Null(data.String);
            Assert.Null(data.Enum);
        }

        [Fact]
        public void IgnoresMultipleValues()
        {
            BinaryReadWriter<MixedData> binaryReadWriter = new();
            MemoryStream memoryStream = new(new byte[]
            {
                // Field index 0
                0, 0,
                // True
                1,
                // Field index 0
                0, 0,
                // False
                0,
                // Field index 0
                0, 0,
                // False
                0
            });

            MixedData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal(true, data.Bool);
        }

        [Fact]
        public void HandlesNullValues()
        {
            BinaryReadWriter<MixedData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new MixedData
            {
                Bool = null,
                Int = null,
                String = null,
                Enum = null
            }, memoryStream);
            memoryStream.Position = 0;

            MixedData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Null(data.Bool);
            Assert.Null(data.Int);
            Assert.Null(data.String);
            Assert.Null(data.Enum);
        }

        [Fact]
        public void ThrowsOnNonNullableData()
        {
            Assert.Throws<InvalidOperationException>(() => new BinaryReadWriter<NonNullableData>());
        }

        [Fact]
        public void ThrowsOnNonGettableData()
        {
            Assert.Throws<InvalidOperationException>(() => new BinaryReadWriter<NonGettableData>());
        }

        [Fact]
        public void ThrowsOnNonSettableData()
        {
            Assert.Throws<InvalidOperationException>(() => new BinaryReadWriter<NonSettableData>());
        }

        [Fact]
        public void ThrowsOnNonDuplicateFieldIndex()
        {
            Assert.Throws<InvalidOperationException>(() => new BinaryReadWriter<DuplicateFieldIndexData>());
        }

        private class BoolData
        {
            [FieldIndex(ushort.MaxValue)]
            public bool? Value { get; set; }
        }

        private class IntData
        {
            [FieldIndex(ushort.MaxValue)]
            public int? Value { get; set; }
        }

        private enum Enum
        {
            A,
            B,
            C
        }

        private class EnumData
        {
            [FieldIndex(0)]
            public Enum? Value { get; set; }
        }

        private class Utf8StringData
        {
            [Encoding(EncodingType.Utf8)]
            [FieldIndex(ushort.MaxValue)]
            public string? Value { get; set; }
        }

        private class Utf16StringData
        {
            [Encoding(EncodingType.Utf16)]
            [FieldIndex(ushort.MaxValue)]
            public string? Value { get; set; }
        }

        private class MultipleData
        {
            [FieldIndex(0)]
            public int? Value0 { get; set; }

            [FieldIndex(1)]
            public int? Value1 { get; set; }

            [FieldIndex(2)]
            public int? Value2 { get; set; }
        }

        private class MixedData
        {
            [FieldIndex(0)]
            public bool? Bool { get; set; }

            [FieldIndex(1)]
            public int? Int { get; set; }

            [FieldIndex(2)]
            public string? String { get; set; }

            [FieldIndex(3)]
            public Enum? Enum { get; set; }
        }

        private class ArrayData
        {
            [FieldIndex(0)]
            public MixedData[]? Array { get; set; }
        }

        private class NestedData
        {
            [FieldIndex(0)]
            public MixedData MixedData { get; set; }

            [FieldIndex(1)]
            public MultipleData NullField { get; set; }

            [FieldIndex(2)]
            public MultipleData MultipleData { get; set; }
        }

        private class PartiallyUnannotatedData
        {
            [FieldIndex(0)]
            public bool? Bool { get; set; }

            public int? Int { get; set; }

            [FieldIndex(2)]
            public string? String { get; set; }
        }

        private class NonNullableData
        {
            [FieldIndex(0)]
            public bool Value { get; set; }
        }

        private class NonSettableData
        {
            [FieldIndex(0)]
            public bool? Bool { get; }
        }

        private class NonGettableData
        {
            [FieldIndex(0)]
            public bool? Value
            {
                set { }
            }
        }

        private class DuplicateFieldIndexData
        {
            [FieldIndex(0)]
            public string Value0 { get; set; }

            [FieldIndex(0)]
            public string Value1 { get; set; }
        }
    }
}