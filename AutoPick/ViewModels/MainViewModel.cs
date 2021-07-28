namespace AutoPick.ViewModels
{
    using System;
    using System.Drawing;
    using System.Windows.Media.Imaging;
    using AutoPick.StateDetection.Definition;

    public class MainViewModel : ViewModelBase, IUserConfiguration, IDetectionInfoConsumer
    {
        public MainViewModel(BitmapSource screenshotPreviewSource, Champion[] champions)
        {
            ScreenshotPreviewSource = screenshotPreviewSource;
            Champions = champions;
            _selectedChampion = champions[0];
        }

        public ViewLayout[] ViewLayouts { get; } = Enum.GetValues<ViewLayout>();

        private ViewLayout _viewLayout = ViewLayout.Tiny;
        public ViewLayout ViewLayout
        {
            get => _viewLayout;

            set
            {
                if (_viewLayout == value)
                {
                    return;
                }

                _viewLayout = value;
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

        public Champion[] Champions { get; }

        private Champion _selectedChampion;
        public Champion SelectedChampion
        {
            get => _selectedChampion;

            set
            {
                if (value == null!)
                {
                    return;
                }

                if (_selectedChampion == value)
                {
                    return;
                }

                _selectedChampion = value;
                NotifyPropertyChanged();
            }
        }

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

        string IUserConfiguration.ChampionName => SelectedChampion.Name;

        public void Consume(DetectionInfo detectionInfo)
        {
            DetectionInfo = detectionInfo;
        }
    }
}