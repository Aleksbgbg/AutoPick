namespace AutoPick
{
    using System;
    using System.Drawing;
    using AutoPick.StateDetection.Definition;

    public class DetectionInfo
    {
        public DetectionInfo(State state, Size windowSize)
        {
            WindowAvailable = state.IsWindowAvailable();
            State = state;
            WindowSize = windowSize;
        }

        public bool WindowAvailable { get; }

        public State State { get; }

        public Size WindowSize { get; }

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

            return Equals((DetectionInfo)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)State, WindowSize);
        }

        public static bool operator ==(DetectionInfo? left, DetectionInfo? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DetectionInfo? left, DetectionInfo? right)
        {
            return !Equals(left, right);
        }

        private bool Equals(DetectionInfo other)
        {
            return State == other.State && WindowSize.Equals(other.WindowSize);
        }
    }
}