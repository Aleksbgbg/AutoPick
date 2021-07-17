namespace AutoPick.Tests.Unit
{
    using System;
    using System.IO;
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
        public void SerializesString()
        {
            BinaryReadWriter<StringData> binaryReadWriter = new();
            MemoryStream memoryStream = new();

            binaryReadWriter.Serialize(new StringData { Value = "abcdef" }, memoryStream);

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
        public void DeserializesString()
        {
            BinaryReadWriter<StringData> binaryReadWriter = new();
            MemoryStream memoryStream = new();
            binaryReadWriter.Serialize(new StringData { Value = "hello" }, memoryStream);
            memoryStream.Position = 0;

            StringData data = binaryReadWriter.Deserialize(memoryStream);

            Assert.Equal("hello", data.Value);
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

        [Fact]
        public void HandlesRandomData()
        {
            BinaryReadWriter<MixedData> binaryReadWriter = new();
            MemoryStream memoryStream = new(new byte[]
            {
                0, 0, 231, 242, 149, 10, 5, 218, 8, 51, 7, 176, 205, 65, 93, 34, 252, 244, 99, 150
            });

            binaryReadWriter.Deserialize(memoryStream);
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

        private class StringData
        {
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