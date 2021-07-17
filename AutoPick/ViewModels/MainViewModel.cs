namespace AutoPick.ViewModels
{
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using AutoPick.StateDetection.Definition;

    public class MainViewModel : ViewModelBase, IUserConfiguration, IDetectionInfoConsumer
    {
        public MainViewModel(BitmapSource screenshotPreviewSource)
        {
            ScreenshotPreviewSource = screenshotPreviewSource;
        }

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

        private DetectionInfo _detectionInfo = new(State.NotLaunched, Size.Empty);
        public DetectionInfo DetectionInfo
        {
            get => _detectionInfo;

            private set
            {
                if (_detectionInfo == value)
                {
                    return;
                }

                _detectionInfo = value;
                NotifyPropertyChanged();
            }
        }

        public BitmapSource ScreenshotPreviewSource { get; }

        string IUserConfiguration.LaneName => LaneText;

        string IUserConfiguration.ChampionName => ChampText;

        public void Consume(DetectionInfo detectionInfo)
        {
            DetectionInfo = detectionInfo;
        }
    }
}