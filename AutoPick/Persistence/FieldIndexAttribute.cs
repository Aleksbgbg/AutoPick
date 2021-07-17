namespace AutoPick.Persistence
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class FieldIndexAttribute : Attribute
    {
        public FieldIndexAttribute(ushort fieldIndex)
        {
            FieldIndex = fieldIndex;
        }

        public ushort FieldIndex { get; }
    }
}