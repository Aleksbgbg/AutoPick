namespace AutoPick.StateDetection.Definition
{
    using System;

    [AttributeUsage(AttributeTargets.Field)]
    public class ZOrderAttribute : Attribute
    {
        public ZOrderAttribute(int zOrder)
        {
            ZOrder = zOrder;
        }

        public int ZOrder { get; }
    }
}