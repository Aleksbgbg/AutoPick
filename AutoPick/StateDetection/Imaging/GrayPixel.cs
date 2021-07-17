namespace AutoPick.StateDetection.Imaging
{
    using Emgu.CV.Structure;

    public class GrayPixel : IPixel
    {
        private readonly Gray _gray;

        public GrayPixel(Gray gray)
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

            return Equals((GrayPixel)obj);
        }

        private bool Equals(GrayPixel other)
        {
            return _gray.Equals(other._gray);
        }
    }
}