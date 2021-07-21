namespace AutoPick.ViewModels
{
    using System;
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

        public Lane[] Lanes => Enum.GetValues<Lane>();

        private Lane _selectedLane;
        public Lane SelectedLane
        {
            get => _selectedLane;

            set
            {
                if (_selectedLane == value)
                {
                    return;
                }

                _selectedLane = value;
                NotifyPropertyChanged();
            }
        }

        Lane IUserConfiguration.Lane => SelectedLane;

        string IUserConfiguration.ChampionName => ChampText;

        public void Consume(DetectionInfo detectionInfo)
        {
            DetectionInfo = detectionInfo;
        }
    }
}