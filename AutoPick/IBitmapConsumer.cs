namespace AutoPick
{
    using System.Drawing;

    public interface IBitmapConsumer
    {
        void Consume(Bitmap bitmap);
    }
}