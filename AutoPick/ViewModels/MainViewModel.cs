namespace AutoPick.ViewModels
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Windows.Media.Imaging;
    using AutoPick.StateDetection.Definition;
    using AutoPick.Util;

    public class MainViewModel : ViewModelBase, IUserConfiguration, IStateConsumer, IBitmapConsumer
    {
        private string _champText = string.Empty;
        public string ChampText
        {
            get => _champText;

            set
            {
                if (_champText == value)
                {
                    return;
                }

                _champText = value;
                NotifyPropertyChanged();
            }
        }

        private string _laneText = string.Empty;
        public string LaneText
        {
            get => _laneText;

            set
            {
                if (_laneText == value)
                {
                    return;
                }

                _laneText = value;
                NotifyPropertyChanged();
            }
        }

        private State _state;
        public State State
        {
            get => _state;

            private set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                NotifyPropertyChanged();
            }
        }

        private BitmapImage? _source;
        public BitmapImage? Source
        {
            get => _source;

            private set
            {
                if (_source == value)
                {
                    return;
                }

                _source = value;
                NotifyPropertyChanged();
            }
        }

        string IUserConfiguration.LaneName => LaneText;

        string IUserConfiguration.ChampionName => ChampText;

        public void Consume(State state)
        {
            State = state;
        }

        public void Consume(Bitmap bitmap)
        {
            Execute.OnUiThread(() =>
            {
                using MemoryStream memory = new();

                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;

                BitmapImage bitmapImage = new();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                Source = bitmapImage;
            });
        }
    }
}