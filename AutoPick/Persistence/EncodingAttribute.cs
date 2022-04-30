namespace AutoPick.Persistence
{
    using System;
    using System.Text;

    public enum EncodingType
    {
        Utf8,
        Utf16,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class EncodingAttribute : Attribute
    {
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        public EncodingAttribute(EncodingType encodingType)
        {
            Encoding = EncodingFromType(encodingType);
        }

        public Encoding Encoding { get; }

        private static Encoding EncodingFromType(EncodingType encodingType)
        {
            switch (encodingType)
            {
            case EncodingType.Utf8:
                return Encoding.UTF8;
            case EncodingType.Utf16:
                return Encoding.Unicode;
            default:
                return DefaultEncoding;
            }
        }
    }
}