namespace AutoPick.Persistence.Types
{
    using System.Reflection;

    public class PropertySerializationInfo
    {
        public PropertySerializationInfo(PropertyInfo propertyInfo, IReadWriteableType readWriteable)
        {
            PropertyInfo = propertyInfo;
            ReadWriteable = readWriteable;
        }

        public PropertyInfo PropertyInfo { get; }

        public IReadWriteableType ReadWriteable { get; }
    }
}