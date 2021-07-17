namespace AutoPick.StateDetection.Imaging
{
    using Emgu.CV.Structure;

    public class EmguCvGrayPixel : IPixel
    {
        private readonly Gray _gray;

        public EmguCvGrayPixel(Gray gray)
        {
            _gray = gray;
        }

        public override int GetHashCode()
        {
            return _gray.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((EmguCvGrayPixel)obj);
        }

        private bool Equals(EmguCvGrayPixel other)
        {
            return _gray.Equals(other._gray);
        }
    }
}